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
using MyHeart.Data.Models.ProfessionInformation;
using MyHeart.DTO.ProfessionInformation;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MyHeart.DTO;
using System;

namespace MyHeart.Services
{
    public class MedicamentGroupService : IMedicamentGroupService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MyHeartContext _myHeartContext;
        private readonly IConfiguration _configuration;

        private readonly IEmailService _emailService;

        public MedicamentGroupService(IHttpContextAccessor httpContextAccessor, MyHeartContext gradologyContext, IMapper mapper, IConfiguration configuration, IEmailService emailService)
        {
            _httpContextAccessor = httpContextAccessor;
            _myHeartContext = gradologyContext;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<IEnumerable<MedicamentGroupDTO>> MedicamentGroupListAsync()
        {
            var medicamentGroup = await _myHeartContext.MedicamentGroup.ToListAsync();

            return _mapper.Map<IEnumerable<MedicamentGroup>, IEnumerable<MedicamentGroupDTO>>(medicamentGroup);
        }

        public async Task<DataTableResponse<MedicamentGroupDTO>> GetMedicamentGroupsTable(DataTableRequest request) {
            List<MedicamentGroup> medicamentGroups;
            int totalCount;

            //no need to search all strings if there is no filter
            if (!string.IsNullOrEmpty(request.Filter)) {
                //no need to filter twice
                var filtered = _myHeartContext.MedicamentGroup
                    .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()));

                medicamentGroups = await filtered
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                totalCount = await filtered
                    .CountAsync();
            } else {
                medicamentGroups = await _myHeartContext.MedicamentGroup
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                totalCount = await _myHeartContext.MedicamentGroup
                    .CountAsync();
            }

            return new DataTableResponse<MedicamentGroupDTO> {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<MedicamentGroup>, IEnumerable<MedicamentGroupDTO>>(medicamentGroups).ToList()
            };
        }

        public async Task<DataTableResponse<MedicamentGroupDTO>> GetPagedResource(SortedPagedSourceRequest request) {
            List<MedicamentGroup> groups;
            int totalCount;
            var selected = request.Selected.ToList();
            var clientPages = (int)Math.Ceiling((double)selected.Count / request.PageSize);
            var firstPageOffset = selected.Count % request.PageSize;

            var group = _myHeartContext.MedicamentGroup.AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter)) {
                group = group
                    .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()));

                groups = await group
                        .Skip((request.Page - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync();

                totalCount = (await group.CountAsync());
            } else {
                if (request.Selected.Count() > 0) {
                    group = group
                        .Where(x => !selected.Contains(x.Id));
                }

                if (request.Page < clientPages || (request.Page == clientPages && firstPageOffset == 0)) {
                    groups = new List<MedicamentGroup>();
                } else if (request.Page == clientPages) {
                    groups = await group
                        .Take(request.PageSize - firstPageOffset)
                        .ToListAsync();
                } else {
                    groups = await group
                        .Skip((request.Page - clientPages - 1) * request.PageSize + (clientPages == 0 ? 0 : (request.PageSize - firstPageOffset)))
                        .Take(request.PageSize)
                        .ToListAsync();
                }

                totalCount = (await group.CountAsync()) + selected.Count;
            }

            return new DataTableResponse<MedicamentGroupDTO> {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<MedicamentGroup>, IEnumerable<MedicamentGroupDTO>>(groups).ToList()
            };
        }

        public async Task<IEnumerable<MedicamentGroupDTO>> GetByIds(IEnumerable<int> ids) {
            var groups = await _myHeartContext.MedicamentGroup
                .Where(x => ids.Contains(x.Id)).ToListAsync();

            return _mapper.Map<IEnumerable<MedicamentGroup>, IEnumerable<MedicamentGroupDTO>>(groups);
        }
        public async Task<MedicamentGroupDTO> MedicamentGroupAsync(int medicamentGroupId)
        {
            var medicamentGroup = await _myHeartContext.MedicamentGroup
                .Where(d => d.Id == medicamentGroupId)
                .Include(x => x.SideEffects)
                .Include(p => p.Indication)
                .Include(p => p.Contraindication)
                .Include(p => p.ActiveSubstance)
                .Include(p => p.Atcs).ThenInclude(x => x.Atc)
                .FirstOrDefaultAsync();

            var result = _mapper.Map<MedicamentGroup, MedicamentGroupDTO>(medicamentGroup);

            var cachedDiscards = _myHeartContext.MedicamentGroup_Pharmacy_Discard
                .Where(x => x.MedicamentGroupId == medicamentGroupId)
                .Select(x => x.Pharmacy.Id)
                .ToList();

            var atcs = _myHeartContext.MedicamentGroup_Atc
                .Where(x => x.MedicamentGroupId == medicamentGroupId)
                .Select(x => x.Atc.AtcCode)
                .ToList();

            result.Pharmacies = _myHeartContext.Pharmacy
                .Where(x => atcs.Contains(x.AtcWho))
                .Where(x => !cachedDiscards.Contains(x.Id))
                .Select(x => new PharmacyDTO
                {
                    Id = x.Id,
                    NameReg = x.NameReg
                })
                .ToList();

            return result;
        }

        public async Task<MedicamentGroupDTO> UpdateMedicamentGroup(MedicamentGroupDTO medicamentGroup, string user)
        {
            var dbMedicamentGroup = await _myHeartContext.MedicamentGroup
                .Where(d => d.Id == medicamentGroup.Id)
                .Include(x => x.SideEffects)
                .Include(p => p.Indication)
                .Include(p => p.Contraindication)
                .Include(p => p.ActiveSubstance)
                .Include(p => p.Atcs)
                .FirstOrDefaultAsync();

            if (dbMedicamentGroup == null)
            {
                return null;
            }

            dbMedicamentGroup.Name = medicamentGroup.Name;
            dbMedicamentGroup.Description = medicamentGroup.Description;
            dbMedicamentGroup.LastUpdateUser = user;

            #region pharmacy bond
            int bondCount = dbMedicamentGroup.ActiveSubstance.Count;
            List<MedicamentGroup_Pharmacy_ActiveSubstance> pharmacyList = new List<MedicamentGroup_Pharmacy_ActiveSubstance>();
            List<int> idList = new List<int>();
            var lastPharmacy = await _myHeartContext.MedicamentGroup_Pharmacy_ActiveSubstance.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastPharmacy == null ? 0 : lastPharmacy.Id;
            int itemId = 1;
            foreach (var item in medicamentGroup.ActiveSubstance)
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

                    bool exist = false;
                    foreach (var existItem in dbMedicamentGroup.ActiveSubstance)
                    {
                        if (existItem.PharmacyId == item.PharmacyId && existItem.MedicamentGroupId == item.MedicamentGroupId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        pharmacyList.Add(_mapper.Map<MedicamentGroup_Pharmacy_ActiveSubstance>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<MedicamentGroup_Pharmacy_ActiveSubstance>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.MedicamentGroup_Pharmacy_ActiveSubstance.AddAsync(bond);
                        idList.Add(item.Id);
                    }


                }
                else
                {
                    pharmacyList.Add(_mapper.Map<MedicamentGroup_Pharmacy_ActiveSubstance>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbMedicamentGroup.ActiveSubstance)
                {
                    foreach (var nonpharma in pharmacyList)
                    {
                        if (nonpharma.MedicamentGroupId == item.MedicamentGroupId && nonpharma.PharmacyId == item.PharmacyId)
                        {
                            //item.Id = symptom.Id;
                            item.BondStrength = nonpharma.BondStrength;
                            item.LastUpdateUser = user;
                        }

                    }

                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.MedicamentGroup_Pharmacy_ActiveSubstance.Remove(item);
                    }
                }
            }
            #endregion

            #region contraindication bond
            bondCount = dbMedicamentGroup.Contraindication.Count;
            idList = new List<int>();
            var lastContraIndication = await _myHeartContext.MedicamentGroup_Disease_Contraindication.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastContraIndication == null ? 0 : lastContraIndication.Id;
            itemId = 1;
            foreach (var item in medicamentGroup.Contraindication)
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
                    var bond = _mapper.Map<MedicamentGroup_Disease_Contraindication>(item);
                    bond.LastUpdateUser = user;
                    await _myHeartContext.MedicamentGroup_Disease_Contraindication.AddAsync(bond);
                    idList.Add(item.Id);
                }
                else
                {
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbMedicamentGroup.Contraindication)
                {
                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.MedicamentGroup_Disease_Contraindication.Remove(item);
                    }
                }
            }
            #endregion

            #region indication bond
             bondCount = dbMedicamentGroup.Indication.Count;
            idList = new List<int>();
            List<MedicamentGroup_Disease_Indication> indicationList = new List<MedicamentGroup_Disease_Indication>();
            var lastIndication = await _myHeartContext.MedicamentGroup_Disease_Indication.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastIndication == null ? 0 : lastIndication.Id;
            itemId = 1;
            foreach (var item in medicamentGroup.Indication)
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
                    foreach (var existItem in dbMedicamentGroup.Indication)
                    {
                        if (existItem.DiseaseId == item.DiseaseId && existItem.MedicamentGroupId == item.MedicamentGroupId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        indicationList.Add(_mapper.Map<MedicamentGroup_Disease_Indication>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<MedicamentGroup_Disease_Indication>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.MedicamentGroup_Disease_Indication.AddAsync(bond);
                        idList.Add(item.Id);
                    }


                }
                else
                {
                    indicationList.Add(_mapper.Map<MedicamentGroup_Disease_Indication>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbMedicamentGroup.Indication)
                {
                    foreach (var indication in indicationList)
                    {
                        if (indication.DiseaseId == item.DiseaseId && indication.MedicamentGroupId == item.MedicamentGroupId)
                        {
                            item.BondStrength = indication.BondStrength;
                            item.LastUpdateUser = user;
                        }
                    }

                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.MedicamentGroup_Disease_Indication.Remove(item);
                    }
                }
            }
            #endregion

            #region sideEffects bond
            bondCount = dbMedicamentGroup.SideEffects.Count;
            idList = new List<int>();
            List<MedicamentGroup_Symptoms_SideEffects> sideEffectList = new List<MedicamentGroup_Symptoms_SideEffects>();
            var lastSideEffect = await _myHeartContext.MedicamentGroup_Symptoms_SideEffects.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastSideEffect == null ? 0 : lastSideEffect.Id;
            itemId = 1;
            foreach (var item in medicamentGroup.SideEffects)
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

                    bool exist = false;
                    foreach (var existItem in dbMedicamentGroup.SideEffects)
                    {
                        if (existItem.SymptomsId == item.SymptomsId && existItem.MedicamentGroupId == item.MedicamentGroupId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        sideEffectList.Add(_mapper.Map<MedicamentGroup_Symptoms_SideEffects>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<MedicamentGroup_Symptoms_SideEffects>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.MedicamentGroup_Symptoms_SideEffects.AddAsync(bond);
                        idList.Add(item.Id);
                    }

                }
                else
                {
                    sideEffectList.Add(_mapper.Map<MedicamentGroup_Symptoms_SideEffects>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbMedicamentGroup.SideEffects)
                {
                    foreach (var sideEffect in sideEffectList)
                    {
                        if (sideEffect.SymptomsId == item.SymptomsId && sideEffect.MedicamentGroupId == item.MedicamentGroupId)
                        {
                            item.BondStrength = sideEffect.BondStrength;
                            item.LastUpdateUser = user;
                        }
                    }

                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.MedicamentGroup_Symptoms_SideEffects.Remove(item);
                    }
                }
            }
            #endregion

            #region atc bond
            bondCount = dbMedicamentGroup.Atcs.Count;
            idList = new List<int>();
            var lastAtc = await _myHeartContext.MedicamentGroup_Atc.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastAtc == null ? 0 : lastAtc.Id;
            itemId = 1;
            foreach(var item in medicamentGroup.Atcs)
            {
                if(item.Id == 0)
                {
                    var found = dbMedicamentGroup.Atcs.Where(x => x.MedicamentGroupId == item.Id && x.AtcId == x.AtcId).FirstOrDefault();
                    if (found != null) {
                        idList.Add(found.Id);
                        continue;
                    }

                    if(lastAtc != null)
                    {
                        lastId++;
                        item.Id = lastId;
                    }
                    else
                    {
                        item.Id = itemId;
                        itemId++;
                    }
                    var bond = _mapper.Map<MedicamentGroup_Atc>(item);
                    bond.LastUpdateUser = user;
                    await _myHeartContext.MedicamentGroup_Atc.AddAsync(bond);
                    idList.Add(item.Id);
                }
                else
                {
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbMedicamentGroup.Atcs)
                {
                    if(!idList.Contains(item.Id)){
                        _myHeartContext.MedicamentGroup_Atc.Remove(item);
                    }
                }
            }
            #endregion
            
            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<MedicamentGroupDTO>(medicamentGroup);
        }

        public async Task<MedicamentGroupDTO> AddMedicamentGroupAsync(MedicamentGroupDTO medicamentGroup, string user)
        {
            var dbMedicamentGroup = new MedicamentGroup() {
                Name = medicamentGroup.Name,
                Description = medicamentGroup.Description,
                LastUpdateUser = user
            };

            _myHeartContext.Add(dbMedicamentGroup);

            await _myHeartContext.SaveChangesAsync();


            #region pharmacy bond
            List<int> idList = new List<int>();
            var lastPharmacy = await _myHeartContext.MedicamentGroup_Pharmacy_ActiveSubstance.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastPharmacy == null ? 0 : lastPharmacy.Id;
            int itemId = 1;
            if (medicamentGroup.ActiveSubstance != null)
            {
                foreach (var item in medicamentGroup.ActiveSubstance)
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
                        item.MedicamentGroupId = dbMedicamentGroup.Id;
                        var bond = _mapper.Map<MedicamentGroup_Pharmacy_ActiveSubstance>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.MedicamentGroup_Pharmacy_ActiveSubstance.AddAsync(bond);
                    }
                }
            }
            
            #endregion

            #region contraindication bond
            var lastContraIndication = await _myHeartContext.MedicamentGroup_Disease_Contraindication.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastContraIndication == null ? 0 : lastContraIndication.Id;
            itemId = 1;
            if (medicamentGroup.Contraindication != null)
            {
                foreach (var item in medicamentGroup.Contraindication)
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
                        item.MedicamentGroupId = dbMedicamentGroup.Id;
                        var bond = _mapper.Map<MedicamentGroup_Disease_Contraindication>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.MedicamentGroup_Disease_Contraindication.AddAsync(bond);
                    }
                }
            }
            
            #endregion

            #region indication bond
            var lastIndication = await _myHeartContext.MedicamentGroup_Disease_Indication.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastIndication == null ? 0 : lastIndication.Id;
            itemId = 1;
            if (medicamentGroup.Indication != null)
            {
                foreach (var item in medicamentGroup.Indication)
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
                        item.MedicamentGroupId = dbMedicamentGroup.Id;
                        var bond = _mapper.Map<MedicamentGroup_Disease_Indication>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.MedicamentGroup_Disease_Indication.AddAsync(bond);
                    }
                }
            }
            
            #endregion

            #region sideEffects bond
            var lastSideEffect = await _myHeartContext.MedicamentGroup_Symptoms_SideEffects.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastSideEffect == null ? 0 : lastSideEffect.Id;
            itemId = 1;
            if (medicamentGroup.SideEffects != null)
            {
                foreach (var item in medicamentGroup.SideEffects)
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
                        item.MedicamentGroupId = dbMedicamentGroup.Id;
                        var bond = _mapper.Map<MedicamentGroup_Symptoms_SideEffects>(item);
                        await _myHeartContext.MedicamentGroup_Symptoms_SideEffects.AddAsync(bond);
                    }
                }
            }

            #endregion

            #region atc bond
            var lastAtc = await _myHeartContext.MedicamentGroup_Atc.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastAtc == null ? 0 : lastAtc.Id;
            itemId = 1;
            if (medicamentGroup.Atcs != null)
            {
                foreach (var item in medicamentGroup.Atcs)
                {
                    if (item.Id == 0)
                    {
                        if (lastAtc != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.MedicamentGroupId = dbMedicamentGroup.Id;
                        var bond = _mapper.Map<MedicamentGroup_Atc>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.MedicamentGroup_Atc.AddAsync(bond);
                    }
                }
            }
            #endregion

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<MedicamentGroup, MedicamentGroupDTO>(dbMedicamentGroup); 
        }

        public async Task<MedicamentGroupDTO> DeleteMedicamentGroup(int medicamentGroupId)
        {
            var dbmedicamentGroup = await _myHeartContext.MedicamentGroup.FirstOrDefaultAsync(u => u.Id == medicamentGroupId);

            if (dbmedicamentGroup == null)
            {
                return null;
            }

            _myHeartContext.MedicamentGroup.Remove(dbmedicamentGroup);

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<MedicamentGroupDTO>(dbmedicamentGroup);


        }
        public async Task<Dictionary<string, string>> ValidateMedicamentGroup(MedicamentGroupDTO therapyNews)
        {
            var errorList = new Dictionary<string, string>();

            return errorList;
        }







        public async Task<IEnumerable<PharmacyDTO>> GetExcludedPharmacies(int medicamentGroupId)
        {
            var pharmacies = _myHeartContext.MedicamentGroup_Pharmacy_Discard
                .Where(x => x.MedicamentGroupId == medicamentGroupId)
                .Select(x => x.Pharmacy);

            return _mapper.Map<IEnumerable<Pharmacy>, IEnumerable<PharmacyDTO>>(pharmacies);
        }

        public async Task<DiscardedPharmaciesResponse<MedicamentGroup_Pharmacy_ByAtc>> GetDiscardedPharmacies(DiscardedPharmaciesRequest request)
        {
            var atcCodes = await _myHeartContext.MedicamentGroup_Atc
                .Where(x => x.MedicamentGroupId == request.Group)
                .Select(x => x.Atc.AtcCode)
                .ToListAsync();

            var cachedDiscardIds = await _myHeartContext.MedicamentGroup_Pharmacy_Discard
                .Where(x => x.MedicamentGroupId == request.Group)
                .Select(x => x.PharmacyId)
                .ToListAsync();

            var pharmacies = _myHeartContext.Pharmacy
                .Where(x => atcCodes.Contains(x.AtcWho));

            var mgpa = await pharmacies
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new MedicamentGroup_Pharmacy_ByAtc {
                    Id = x.Id,
                    MedicamentGroupId = request.Group,
                    Name = x.NameReg + " - " + x.Supplement,
                    PharmacyId = x.Id,
                    Discarded = cachedDiscardIds.Contains(x.Id)
                })
                .ToListAsync();

            return new DiscardedPharmaciesResponse<MedicamentGroup_Pharmacy_ByAtc> { 
                Group = request.Group,
                PageSize = request.PageSize,
                Page = request.PageSize,
                TotalCount = await pharmacies.CountAsync(),
                Data = mgpa
            };
        }

        public async Task<MedicamentGroup_Pharmacy_ByAtc> TogglePharmacyDiscard(MedicamentGroup_Pharmacy_ByAtc pharmacy, string user)
        {
            var dbModel = _myHeartContext.MedicamentGroup_Pharmacy_Discard
                .FirstOrDefault(x => x.PharmacyId == pharmacy.PharmacyId && x.MedicamentGroupId == pharmacy.MedicamentGroupId);

            if(dbModel == null)
            {
                _myHeartContext.MedicamentGroup_Pharmacy_Discard
                    .Add(new MedicamentGroup_Pharmacy_Discard {
                        MedicamentGroupId = pharmacy.MedicamentGroupId,
                        PharmacyId = pharmacy.PharmacyId,
                        LastUpdateUser = user
                    });
                await _myHeartContext.SaveChangesAsync();
                return pharmacy;
            }
            else
            {
                _myHeartContext.MedicamentGroup_Pharmacy_Discard
                    .Remove(dbModel);
                await _myHeartContext.SaveChangesAsync();
                return pharmacy;
            }
        }
    }
}