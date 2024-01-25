using AutoMapper;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO;
using System;
using EFCore.BulkExtensions;
using MoreLinq;
using MyHeart.DTO.User;

namespace MyHeart.Services
{
    public class PharmacyService : IPharmacyService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MyHeartContext _myHeartContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public PharmacyService(IHttpContextAccessor httpContextAccessor, MyHeartContext gradologyContext, IMapper mapper, IConfiguration configuration, IEmailService emailService)
        {
            _httpContextAccessor = httpContextAccessor;
            _myHeartContext = gradologyContext;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<IEnumerable<PharmacyDTO>> PharmacyListAsync()
        {
            var pharmacy = await _myHeartContext.Pharmacy.ToListAsync();

            return _mapper.Map<IEnumerable<Pharmacy>, IEnumerable<PharmacyDTO>>(pharmacy);
        }

        public async Task<DataTableResponse<PharmacyDTO>> GetPharmaciesTable(DataTableRequest request)
        {
            var pharmacies = await _myHeartContext.Pharmacy
                .Where(x => x.NameReg.ToLower().Contains(request.Filter.ToLower()))
                .Where(x => x.Strength.ToLower().Contains(request.SecondFilter.ToLower()))
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var totalCount = _myHeartContext.Pharmacy
                .Where(x => x.NameReg.ToLower().Contains(request.Filter.ToLower()))
                .Count();

            return new DataTableResponse<PharmacyDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<Pharmacy>, IEnumerable<PharmacyDTO>>(pharmacies).ToList()
            };
        }

        public async Task<DataTableResponse<PharmacyDTO>> GetPagedResource(SortedPagedSourceRequest request) {
            List<Pharmacy> symptoms;
            int totalCount;
            var selected = request.Selected.ToList();
            var clientPages = (int)Math.Ceiling((double)selected.Count / request.PageSize);
            var firstPageOffset = selected.Count % request.PageSize;

            var dis = _myHeartContext.Pharmacy.AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter)) {
                dis = dis
                    .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()));

                symptoms = await dis
                        .Skip((request.Page - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync();

                totalCount = (await dis.CountAsync());
            } else {
                if (request.Selected.Count() > 0) {
                    dis = dis
                        .Where(x => !selected.Contains(x.Id));
                }

                if (request.Page < clientPages || (request.Page == clientPages && firstPageOffset == 0)) {
                    symptoms = new List<Pharmacy>();
                } else if (request.Page == clientPages) {
                    symptoms = await dis
                        .Take(request.PageSize - firstPageOffset)
                        .ToListAsync();
                } else {
                    symptoms = await dis
                        .Skip((request.Page - clientPages - 1) * request.PageSize + (clientPages == 0 ? 0 : (request.PageSize - firstPageOffset)))
                        .Take(request.PageSize)
                        .ToListAsync();
                }

                totalCount = (await dis.CountAsync()) + selected.Count;
            }

            

            return new DataTableResponse<PharmacyDTO> {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<Pharmacy>, IEnumerable<PharmacyDTO>>(symptoms).ToList()
            };
        }

        public async Task<IEnumerable<PharmacyDTO>> GetByIds(IEnumerable<int> ids) {
            var pharmacies = await _myHeartContext.Pharmacy
                .Where(x => ids.Contains(x.Id)).ToListAsync();

            return _mapper.Map<IEnumerable<Pharmacy>, IEnumerable<PharmacyDTO>>(pharmacies);
        }

        private IEnumerable<Pharmacy> FilterList(IEnumerable<Pharmacy> list, string filter)
        {
            var result = list.Where(x => x.NameReg.ToLower().Contains(filter.ToLower()));
            return result;
        }

        public async Task<PharmacyDTO> PharmacyAsync(int pharmacyId)
        {
            var pharmacy = await _myHeartContext.Pharmacy
                .Where(d => d.Id == pharmacyId)
                .Include(x => x.SideEffects)
                .Include(p => p.Indication)
                .Include(p => p.ContraIndication)
                .Include(p => p.ActiveSubstance)
                //.Include(p => p.CommercialNames)
                .FirstOrDefaultAsync();

            return _mapper.Map<Pharmacy, PharmacyDTO>(pharmacy);
        }
        public async Task<IEnumerable<PharmacyDTO>> PharmacyAsync(string nameRegPart)
        {
            var pharmacies = await _myHeartContext.Pharmacy
                .Where(p => p.NameReg.ToLower().Contains(nameRegPart.ToLower()))
                .Take(10)
                .Include(p => p.SideEffects)
                .Include(p => p.Indication)
                .Include(p => p.ContraIndication)
                .Include(p => p.ActiveSubstance)
                .Select(p => _mapper.Map<Pharmacy, PharmacyDTO>(p))
                .ToArrayAsync();

            return pharmacies;
        }

        public async Task<PharmacyDTO> UpdatePharmacy(PharmacyDTO pharmacy, string user)
        {
            var dbpharmacy = await _myHeartContext.Pharmacy
                .Where(u => u.Id == pharmacy.Id)
                .Include(x => x.SideEffects)
                .Include(p => p.Indication)
                .Include(p => p.ContraIndication)
                .Include(p => p.ActiveSubstance)
                //.Include(p => p.CommercialNames)
                .FirstOrDefaultAsync();

            if (dbpharmacy == null)
            {
                return null;
            }

            dbpharmacy.Name = pharmacy.Name;
            dbpharmacy.Dosage = pharmacy.Dosage;
            dbpharmacy.MinimumDose = pharmacy.MinimumDose;
            dbpharmacy.MaximumDose = pharmacy.MaximumDose;
            dbpharmacy.LastUpdateUser = user;
            dbpharmacy.NameReg = pharmacy.NameReg;

            #region pharmacy bond
            int bondCount = dbpharmacy.ActiveSubstance == null ? 0 : dbpharmacy.ActiveSubstance.Count;
            List<int> idList = new List<int>();
            var lastPharmacy = await _myHeartContext.MedicamentGroup_Pharmacy_ActiveSubstance.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastPharmacy == null ? 0 : lastPharmacy.Id;
            int itemId = 1;
            foreach (var item in pharmacy.ActiveSubstance)
            {
                if (item.Id == 0)
                {
                    if (lastPharmacy != null)
                    {
                        lastId++;
                        item.Id = lastId;
                    }
                    else
                    {
                        item.Id = itemId;
                        itemId++;
                    }
                    var bond = _mapper.Map<MedicamentGroup_Pharmacy_ActiveSubstance>(item);
                    bond.LastUpdateUser = user;
                    await _myHeartContext.MedicamentGroup_Pharmacy_ActiveSubstance.AddAsync(bond);
                    idList.Add(item.Id);
                }
                else
                {
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbpharmacy.ActiveSubstance)
                {
                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.MedicamentGroup_Pharmacy_ActiveSubstance.Remove(item);
                    }
                }
            }
            #endregion

            #region contraindication bond
            bondCount = dbpharmacy.ContraIndication == null ? 0 : dbpharmacy.ContraIndication.Count;
            List<Pharmacy_Disease_ContraIndication> diseaseList = new List<Pharmacy_Disease_ContraIndication>();
            idList = new List<int>();
            var lastContraIndication = await _myHeartContext.Pharmacy_Disease_ContraIndication.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastContraIndication == null ? 0 : lastContraIndication.Id;
            itemId = 1;
            foreach (var item in pharmacy.ContraIndication)
            {
                if (item.Id == 0)
                {
                    if (lastContraIndication != null)
                    {
                        lastId++;
                        item.Id = lastId;
                    }
                    else
                    {
                        item.Id = itemId;
                        itemId++;
                    }

                    bool exist = false;
                    foreach (var existItem in dbpharmacy.ContraIndication)
                    {
                        if (existItem.DiseaseId == item.DiseaseId && existItem.PharmacyId == item.PharmacyId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        diseaseList.Add(_mapper.Map<Pharmacy_Disease_ContraIndication>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<Pharmacy_Disease_ContraIndication>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.Pharmacy_Disease_ContraIndication.AddAsync(bond);
                        idList.Add(item.Id);
                    }

                }
                else
                {
                    diseaseList.Add(_mapper.Map<Pharmacy_Disease_ContraIndication>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbpharmacy.ContraIndication)
                {
                    foreach (var disease in diseaseList)
                    {
                        if (disease.DiseaseId == item.DiseaseId && disease.PharmacyId == item.PharmacyId)
                        {
                            //item.Id = symptom.Id;
                            item.BondStrength = disease.BondStrength;
                            item.LastUpdateUser = user;
                        }

                    }
                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.Pharmacy_Disease_ContraIndication.Remove(item);
                    }
                }
            }
            #endregion

            #region indication bond
            bondCount = dbpharmacy.Indication == null ? 0 : dbpharmacy.Indication.Count;
            idList = new List<int>();
            List<Pharmacy_Disease_Indication> indicationList = new List<Pharmacy_Disease_Indication>();
            var lastIndication = await _myHeartContext.Pharmacy_Disease_Indication.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastIndication == null ? 0 : lastIndication.Id;
            itemId = 1;
            foreach (var item in pharmacy.Indication)
            {
                if (item.Id == 0)
                {
                    if (lastIndication != null)
                    {
                        lastId++;
                        item.Id = lastId;
                    }
                    else
                    {
                        item.Id = itemId;
                        itemId++;
                    }

                    bool exist = false;
                    foreach (var existItem in dbpharmacy.ContraIndication)
                    {
                        if (existItem.DiseaseId == item.DiseaseId && existItem.PharmacyId == item.PharmacyId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        indicationList.Add(_mapper.Map<Pharmacy_Disease_Indication>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<Pharmacy_Disease_Indication>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.Pharmacy_Disease_Indication.AddAsync(bond);
                        idList.Add(item.Id);
                    }

                }
                else
                {
                    indicationList.Add(_mapper.Map<Pharmacy_Disease_Indication>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbpharmacy.Indication)
                {
                    foreach (var disease in indicationList)
                    {
                        if (disease.DiseaseId == item.DiseaseId && disease.PharmacyId == item.PharmacyId)
                        {
                            //item.Id = symptom.Id;
                            item.BondStrength = disease.BondStrength;
                            item.LastUpdateUser = user;
                        }
                    }

                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.Pharmacy_Disease_Indication.Remove(item);
                    }
                }
            }
            #endregion

            #region sideEffects bond
            bondCount = dbpharmacy.SideEffects == null ? 0 : dbpharmacy.SideEffects.Count;
            idList = new List<int>();
            var lastSideEffect = await _myHeartContext.Pharmacy_Symptoms_SideEffects.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastSideEffect == null ? 0 : lastSideEffect.Id;
            itemId = 1;
            foreach (var item in pharmacy.SideEffects)
            {
                if (item.Id == 0)
                {
                    if (lastSideEffect != null)
                    {
                        lastId++;
                        item.Id = lastId;
                    }
                    else
                    {
                        item.Id = itemId;
                        itemId++;
                    }
                    var bond = _mapper.Map<Pharmacy_Symptoms_SideEffects>(item);
                    bond.LastUpdateUser = user;
                    await _myHeartContext.Pharmacy_Symptoms_SideEffects.AddAsync(bond);
                    idList.Add(item.Id);
                }
                else
                {
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbpharmacy.SideEffects)
                {
                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.Pharmacy_Symptoms_SideEffects.Remove(item);
                    }
                }
            }
            #endregion

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<PharmacyDTO>(dbpharmacy);
        }

        public async Task<PharmacyDTO> AddPharmacyAsync(PharmacyDTO pharmacy, string user)
        {
            var dbpharmacy = new Pharmacy() {
                Name = pharmacy.Name,
                Dosage = pharmacy.Dosage,
                MinimumDose = pharmacy.MinimumDose,
                MaximumDose = pharmacy.MaximumDose,
                LastUpdateUser = user,
                NameReg = pharmacy.NameReg,
            };

            _myHeartContext.Add(dbpharmacy);

            await _myHeartContext.SaveChangesAsync();

            #region pharmacy bond
            var lastPharmacy = await _myHeartContext.MedicamentGroup_Pharmacy_ActiveSubstance.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastPharmacy == null ? 0 : lastPharmacy.Id;
            int itemId = 1;
            if (pharmacy.ActiveSubstance != null)
            {
                foreach (var item in pharmacy.ActiveSubstance)
                {
                    if (item.Id == 0)
                    {
                        if (lastPharmacy != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.PharmacyId = dbpharmacy.Id;
                        var bond = _mapper.Map<MedicamentGroup_Pharmacy_ActiveSubstance>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.MedicamentGroup_Pharmacy_ActiveSubstance.AddAsync(bond);
                    }
                }
            }
            
            #endregion

            #region contraindication bond
            var lastContraIndication = await _myHeartContext.Pharmacy_Disease_ContraIndication.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastContraIndication == null ? 0 : lastContraIndication.Id;
            itemId = 1;
            if (pharmacy.ContraIndication != null)
            {
                foreach (var item in pharmacy.ContraIndication)
                {
                    if (item.Id == 0)
                    {
                        if (lastContraIndication != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.PharmacyId = dbpharmacy.Id;
                        var bond = _mapper.Map<Pharmacy_Disease_ContraIndication>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.Pharmacy_Disease_ContraIndication.AddAsync(bond);
                    }
                }
            }
            
            #endregion

            #region indication bond
            var lastIndication = await _myHeartContext.Pharmacy_Disease_Indication.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastIndication == null ? 0 : lastIndication.Id;
            itemId = 1;

            if(pharmacy.Indication != null)
            {
                foreach (var item in pharmacy.Indication)
                {
                    if (item.Id == 0)
                    {
                        if (lastIndication != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.PharmacyId = dbpharmacy.Id;
                        var bond = _mapper.Map<Pharmacy_Disease_Indication>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.Pharmacy_Disease_Indication.AddAsync(bond);
                    }
                }
            }
            
            #endregion

            #region sideEffects bond
            var lastSideEffect = await _myHeartContext.Pharmacy_Symptoms_SideEffects.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastSideEffect == null ? 0 : lastSideEffect.Id;
            itemId = 1;
            if (pharmacy.SideEffects != null)
            {
                foreach (var item in pharmacy.SideEffects)
                {
                    if (item.Id == 0)
                    {
                        if (lastSideEffect != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.PharmacyId = dbpharmacy.Id;
                        var bond = _mapper.Map<Pharmacy_Symptoms_SideEffects>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.Pharmacy_Symptoms_SideEffects.AddAsync(bond);
                    }
                }
            }
            
            #endregion

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<Pharmacy, PharmacyDTO>(dbpharmacy);
        }

        public async Task<bool> AddOrUpdatePharmacy(PharmacyDTO pharmacy, string user) {
            var dbPharmacy = await _myHeartContext.Pharmacy.Where(x => x.SuklCode == pharmacy.SuklCode).FirstOrDefaultAsync();
            var found = true;

            if(dbPharmacy == null) {
                dbPharmacy = new Pharmacy();
                found = false;
                dbPharmacy.Dosage = "default";
                dbPharmacy.MinimumDose = "default";
                dbPharmacy.MaximumDose = "default";
            }

            dbPharmacy.SuklCode = pharmacy.SuklCode;
            dbPharmacy.Name = pharmacy.Name;
            dbPharmacy.Strength = pharmacy.Strength;
            dbPharmacy.Form = pharmacy.Form;
            dbPharmacy.Package = pharmacy.Package;
            dbPharmacy.Path = pharmacy.Path;
            dbPharmacy.Supplement = pharmacy.Supplement;
            dbPharmacy.AtcWho = pharmacy.AtcWho;
            dbPharmacy.DddamntWho = pharmacy.DddamntWho;
            dbPharmacy.DddunWho = pharmacy.DddunWho;
            dbPharmacy.NameReg = pharmacy.NameReg;
            dbPharmacy.LastUpdateUser = user;

            if (found) {
                _myHeartContext.Update(dbPharmacy);
            } else {
                await _myHeartContext.AddAsync(dbPharmacy);
            }

            await _myHeartContext.SaveChangesAsync();

            return true;
        }

        public async Task<PharmacyDTO> DeletePharmacy(int pharmacyId)
        {
            var dbpharmacy = await _myHeartContext.Pharmacy.FirstOrDefaultAsync(u => u.Id == pharmacyId);

            if (dbpharmacy == null)
            {
                return null;
            }

            _myHeartContext.Pharmacy.Remove(dbpharmacy);

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<PharmacyDTO>(dbpharmacy);
        }

        public async Task<Dictionary<string, string>> ValidatePharmacy(PharmacyDTO therapyNews)
        {
            var errorList = new Dictionary<string, string>();

            /*
            if (therapyNews.ActiveSubstanceName.Length < 2)
                errorList.Add("Text", "Text nesmí být kratší než 2 znaky");
                */

            return errorList;
        }

        public async Task<bool> BulkInsertOrUpdate(List<PharmacyDTO> pharmacies, string user) {
            try {
                var dbPharma = _mapper.Map<List<PharmacyDTO>, List<Pharmacy>>(pharmacies);
                
                foreach(var pharma in dbPharma) {
                    pharma.LastUpdateUser = user;
                }

                var batches = dbPharma.Batch(2000);

                await _myHeartContext.SetIdentityInsertAsync<Pharmacy>(true);
                foreach(var batch in batches) {
                    var done = false;
                    var i = 0;
                    while (!done) {
                        await Task.Delay(TimeSpan.FromMinutes(5 * i));
                        try {
                            await _myHeartContext.BulkInsertOrUpdateAsync(batch.ToList(), new BulkConfig { SqlBulkCopyOptions = Microsoft.Data.SqlClient.SqlBulkCopyOptions.KeepIdentity, BatchSize = 1000, BulkCopyTimeout = 0 });
                            done = true;
                        } catch (Exception) {
                            i++;
                        }
                    }
                }
                await _myHeartContext.SetIdentityInsertAsync<Pharmacy>(false);

                return true;
            } catch(Exception e) {
                System.Diagnostics.Debug.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> BulkDelete(List<PharmacyDTO> pharmacies) {
            try {
                var dbPharmacies = _mapper.Map<List<PharmacyDTO>, List<Pharmacy>>(pharmacies);

                var batches = dbPharmacies.Batch(2000);

                foreach(var batch in batches) {
                    var done = false;
                    var i = 0;
                    while (!done) {
                        await Task.Delay(TimeSpan.FromMinutes(5 * i));
                        try {
                            await _myHeartContext.BulkDeleteAsync(batch.ToList(), new BulkConfig { SqlBulkCopyOptions = Microsoft.Data.SqlClient.SqlBulkCopyOptions.KeepIdentity, BatchSize = 1000, BulkCopyTimeout = 0 });
                            done = true;
                        } catch (Exception) {
                            i++;
                        }
                    }
                }
                
                return true;
            }catch(Exception e) {
                System.Diagnostics.Debug.WriteLine(e);
                return false;
            }
        }


        public async Task<IEnumerable<UserPharmacyDTO>> GetAllUserPharmacy()
        {
            var userDiseaseList = await _myHeartContext.UserPharmacy
                .ToListAsync();

            return userDiseaseList.Select(_mapper.Map<UserPharmacyDTO>);
        }
    }
}