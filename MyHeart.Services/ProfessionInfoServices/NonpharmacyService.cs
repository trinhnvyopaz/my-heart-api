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
using MyHeart.DTO.User;

namespace MyHeart.Services
{
    public class NonpharmacyService : INonpharmacyService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MyHeartContext _myHeartContext;
        private readonly IConfiguration _configuration;

        private readonly IEmailService _emailService;

        public NonpharmacyService(IHttpContextAccessor httpContextAccessor, MyHeartContext gradologyContext, IMapper mapper, IConfiguration configuration, IEmailService emailService)
        {
            _httpContextAccessor = httpContextAccessor;
            _myHeartContext = gradologyContext;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<IEnumerable<NonpharmacyDTO>> NonpharmacyListAsync()
        {
            var nonpharmaticTherapy = await _myHeartContext.NonpharmaticTherapy.ToListAsync();

            return _mapper.Map<IEnumerable<NonpharmaticTherapy>, IEnumerable<NonpharmacyDTO>>(nonpharmaticTherapy);
        }

        public async Task<DataTableResponse<NonpharmacyDTO>> GetNonpharmacyTable(DataTableRequest request)
        {
            List<NonpharmaticTherapy> nonPharmaticTherapy;
            int totalCount;

            //no need to search all strings if there is no filter
            if (!string.IsNullOrEmpty(request.Filter))
            {
                //no need to filter twice
                var filtered = _myHeartContext.NonpharmaticTherapy
                    .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()));

                nonPharmaticTherapy = await filtered
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                totalCount = await filtered
                    .CountAsync();
            }
            else
            {
                nonPharmaticTherapy = await _myHeartContext.NonpharmaticTherapy
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                totalCount = await _myHeartContext.NonpharmaticTherapy
                    .CountAsync();
            }

            return new DataTableResponse<NonpharmacyDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<NonpharmaticTherapy>, IEnumerable<NonpharmacyDTO>>(nonPharmaticTherapy).ToList()
            };
        }

        public async Task<DataTableResponse<NonpharmacyDTO>> GetPagedResource(SortedPagedSourceRequest request)
        {
            List<NonpharmaticTherapy> symptoms;
            int totalCount;
            var selected = request.Selected.ToList();
            var clientPages = (int)Math.Ceiling((double)selected.Count / request.PageSize);
            var firstPageOffset = selected.Count % request.PageSize;

            var nonPharmacies = _myHeartContext.NonpharmaticTherapy.AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter))
            {
                nonPharmacies = nonPharmacies
                    .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()));

                symptoms = await nonPharmacies
                        .Skip((request.Page - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync();

                totalCount = (await nonPharmacies.CountAsync());
            }
            else
            {
                if (request.Selected.Count() > 0 || (request.Page == clientPages && firstPageOffset == 0))
                {
                    nonPharmacies = nonPharmacies
                        .Where(x => !selected.Contains(x.Id));
                }

                if (request.Page < clientPages)
                {
                    symptoms = new List<NonpharmaticTherapy>();
                }
                else if (request.Page == clientPages)
                {
                    symptoms = await nonPharmacies
                        .Take(request.PageSize - firstPageOffset)
                        .ToListAsync();
                }
                else
                {
                    symptoms = await nonPharmacies
                        .Skip((request.Page - clientPages - 1) * request.PageSize + (clientPages == 0 ? 0 : (request.PageSize - firstPageOffset)))
                        .Take(request.PageSize)
                        .ToListAsync();
                }

                totalCount = (await nonPharmacies.CountAsync()) + selected.Count;
            }

            return new DataTableResponse<NonpharmacyDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<NonpharmaticTherapy>, IEnumerable<NonpharmacyDTO>>(symptoms).ToList()
            };
        }

        public async Task<IEnumerable<NonpharmacyDTO>> GetByIds(IEnumerable<int> ids)
        {
            var nonPharmacy = await _myHeartContext.NonpharmaticTherapy
                .Where(x => ids.Contains(x.Id)).ToListAsync();

            return _mapper.Map<IEnumerable<NonpharmaticTherapy>, IEnumerable<NonpharmacyDTO>>(nonPharmacy);
        }

        public async Task<NonpharmacyDTO> NonpharmacyAsync(int nonpharmacyId)
        {
            var pharmacy = await _myHeartContext.NonpharmaticTherapy
                .Where(d => d.Id == nonpharmacyId)
                .Include(x => x.MedicalIntervention)
                .Include(p => p.Indication)
                .Include(p => p.Complication)
                .Include(x => x.Synonyms)
                .FirstOrDefaultAsync();

            return _mapper.Map<NonpharmaticTherapy, NonpharmacyDTO>(pharmacy);
        }

        public async Task<NonpharmacyDTO> UpdateNonpharmacy(NonpharmacyDTO nonpharmaticTherapy, string user)
        {
            var dbnonpharmaticTherapy = await _myHeartContext.NonpharmaticTherapy
                .Where(u => u.Id == nonpharmaticTherapy.Id)
                .Include(x => x.MedicalIntervention)
                .Include(p => p.Indication)
                .Include(p => p.Complication)
                .Include(x => x.Synonyms)
                .FirstOrDefaultAsync();

            if (dbnonpharmaticTherapy == null)
            {
                return null;
            }

            dbnonpharmaticTherapy.Name = nonpharmaticTherapy.Name;
            dbnonpharmaticTherapy.Description = nonpharmaticTherapy.Description;

            decimal? parsedEfficiency = null;
            if (!string.IsNullOrEmpty(nonpharmaticTherapy.Efficiency))
            {
                decimal.TryParse(nonpharmaticTherapy.Efficiency, out decimal efficiency);
                parsedEfficiency = efficiency;
            }

            dbnonpharmaticTherapy.Efficiency = parsedEfficiency;
            dbnonpharmaticTherapy.HospitalizationLength = nonpharmaticTherapy.HospitalizationLength;
            dbnonpharmaticTherapy.LastUpdateUser = user;

            #region contraindication bond
            int bondCount = dbnonpharmaticTherapy.Complication == null ? 0 : dbnonpharmaticTherapy.Complication.Count;
            List<int> idList = new List<int>();
            List<NonpharmaticTherapy_Disease_Contraindication> contraList = new List<NonpharmaticTherapy_Disease_Contraindication>();
            var lastContraIndication = await _myHeartContext.NonpharmaticTherapy_Disease_Contraindication.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastContraIndication == null ? 0 : lastContraIndication.Id;
            int itemId = 1;
            foreach (var item in nonpharmaticTherapy.Complication)
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
                    foreach (var existItem in dbnonpharmaticTherapy.Complication)
                    {
                        if (existItem.DiseaseId == item.DiseaseId && existItem.NonpharmaticTherapyId == item.NonpharmaticTherapyId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        contraList.Add(_mapper.Map<NonpharmaticTherapy_Disease_Contraindication>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<NonpharmaticTherapy_Disease_Contraindication>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.NonpharmaticTherapy_Disease_Contraindication.AddAsync(bond);
                        idList.Add(item.Id);
                    }

                }
                else
                {
                    contraList.Add(_mapper.Map<NonpharmaticTherapy_Disease_Contraindication>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbnonpharmaticTherapy.Complication)
                {
                    foreach (var contraindication in contraList)
                    {
                        if (contraindication.DiseaseId == item.DiseaseId && contraindication.NonpharmaticTherapyId == item.NonpharmaticTherapyId)
                        {
                            //item.Id = symptom.Id;
                            item.BondStrength = contraindication.BondStrength;
                            item.LastUpdateUser = user;
                        }

                    }
                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.NonpharmaticTherapy_Disease_Contraindication.Remove(item);
                    }
                }
            }
            #endregion

            #region indication bond
            bondCount = dbnonpharmaticTherapy.Indication == null ? 0 : dbnonpharmaticTherapy.Indication.Count;
            idList = new List<int>();
            List<NonpharmaticTherapy_Disease_Indication> indicList = new List<NonpharmaticTherapy_Disease_Indication>();
            var lastIndication = await _myHeartContext.NonpharmaticTherapy_Disease_Indication.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastIndication == null ? 0 : lastIndication.Id;
            itemId = 1;
            foreach (var item in nonpharmaticTherapy.Indication)
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
                    foreach (var existItem in dbnonpharmaticTherapy.Indication)
                    {
                        if (existItem.DiseaseId == item.DiseaseId && existItem.NonpharmaticTherapyId == item.NonpharmaticTherapyId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        indicList.Add(_mapper.Map<NonpharmaticTherapy_Disease_Indication>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<NonpharmaticTherapy_Disease_Indication>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.NonpharmaticTherapy_Disease_Indication.AddAsync(bond);
                        idList.Add(item.Id);
                    }

                }
                else
                {
                    indicList.Add(_mapper.Map<NonpharmaticTherapy_Disease_Indication>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbnonpharmaticTherapy.Indication)
                {
                    foreach (var indication in indicList)
                    {
                        if (indication.DiseaseId == item.DiseaseId && indication.NonpharmaticTherapyId == item.NonpharmaticTherapyId)
                        {
                            //item.Id = symptom.Id;
                            item.BondStrength = indication.BondStrength;
                            item.LastUpdateUser = user;
                        }

                    }
                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.NonpharmaticTherapy_Disease_Indication.Remove(item);
                    }
                }
            }
            #endregion

            #region medicalFacilities bond
            bondCount = dbnonpharmaticTherapy.MedicalIntervention == null ? 0 : dbnonpharmaticTherapy.MedicalIntervention.Count;
            idList = new List<int>();
            List<NonpharmaticTherapy_MedicalFacilities_MedicalIntervention> medicalFacilitiesList = new List<NonpharmaticTherapy_MedicalFacilities_MedicalIntervention>();
            var lastSideEffect = await _myHeartContext.NonpharmaticTherapy_MedicalFacilities_MedicalIntervention.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastSideEffect == null ? 0 : lastSideEffect.Id;
            itemId = 1;
            foreach (var item in nonpharmaticTherapy.MedicalIntervention)
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
                    foreach (var existItem in dbnonpharmaticTherapy.MedicalIntervention)
                    {
                        if (existItem.MedicalFacilitiesId == item.MedicalFacilitiesId && existItem.NonpharmaticTherapyId == item.NonpharmaticTherapyId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        medicalFacilitiesList.Add(_mapper.Map<NonpharmaticTherapy_MedicalFacilities_MedicalIntervention>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<NonpharmaticTherapy_MedicalFacilities_MedicalIntervention>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.NonpharmaticTherapy_MedicalFacilities_MedicalIntervention.AddAsync(bond);
                        idList.Add(item.Id);
                    }

                }
                else
                {
                    medicalFacilitiesList.Add(_mapper.Map<NonpharmaticTherapy_MedicalFacilities_MedicalIntervention>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbnonpharmaticTherapy.MedicalIntervention)
                {
                    foreach (var medical in medicalFacilitiesList)
                    {
                        if (medical.MedicalFacilitiesId == item.MedicalFacilitiesId && medical.NonpharmaticTherapyId == item.NonpharmaticTherapyId)
                        {
                            //item.Id = symptom.Id;
                            item.BondStrength = medical.BondStrength;
                            item.LastUpdateUser = user;
                        }

                    }
                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.NonpharmaticTherapy_MedicalFacilities_MedicalIntervention.Remove(item);
                    }
                }
            }
            #endregion

            #region synonyms

            foreach (var dbSynonym in dbnonpharmaticTherapy.Synonyms)
            {
                var exists = nonpharmaticTherapy.Synonyms.Any(x => x.Id == dbSynonym.Id);
                if (!exists)
                {
                    _myHeartContext.Remove(dbSynonym);
                }
            }

            foreach (var synonym in nonpharmaticTherapy.Synonyms)
            {
                if (synonym.Id == 0)
                {
                    var dbSynonym = _mapper.Map<NonpharmaticTherapy_Synonyms>(synonym);
                    dbSynonym.NonpharmaticTherapyId = nonpharmaticTherapy.Id;
                    _myHeartContext.Add(dbSynonym);
                }
                else
                {
                    var existingSynonym = dbnonpharmaticTherapy.Synonyms.FirstOrDefault(x => x.Id == synonym.Id);
                    if (existingSynonym != null)
                    {
                        existingSynonym.Name = synonym.Name;
                    }
                }
            }

            #endregion

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<NonpharmacyDTO>(nonpharmaticTherapy);
        }

        public async Task<NonpharmacyDTO> AddNonpharmacyAsync(NonpharmacyDTO nonpharmaticTherapy, string user)
        {
            decimal? parsedEfficiency = null;
            if (!string.IsNullOrEmpty(nonpharmaticTherapy.Efficiency))
            {
                decimal.TryParse(nonpharmaticTherapy.Efficiency, out decimal efficiency);
                parsedEfficiency = efficiency;
            }

            var dbnonpharmaticTherapy = new NonpharmaticTherapy()
            {
                Name = nonpharmaticTherapy.Name,
                Description = nonpharmaticTherapy.Description,
                Efficiency = parsedEfficiency,
                HospitalizationLength = nonpharmaticTherapy.HospitalizationLength,
                LastUpdateUser = user,
                Synonyms = _mapper.Map<List<NonpharmaticTherapy_Synonyms>>(nonpharmaticTherapy.Synonyms)
            };

            _myHeartContext.Add(dbnonpharmaticTherapy);

            await _myHeartContext.SaveChangesAsync();

            #region contraindication bond
            var lastContraIndication = await _myHeartContext.NonpharmaticTherapy_Disease_Contraindication.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastContraIndication == null ? 0 : lastContraIndication.Id;
            int itemId = 1;
            if (nonpharmaticTherapy.Complication != null)
            {
                foreach (var item in nonpharmaticTherapy.Complication)
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
                        item.NonpharmaticTherapyId = dbnonpharmaticTherapy.Id;
                        var bond = _mapper.Map<NonpharmaticTherapy_Disease_Contraindication>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.NonpharmaticTherapy_Disease_Contraindication.AddAsync(bond);
                    }
                }
            }

            #endregion

            #region indication bond
            var lastIndication = await _myHeartContext.NonpharmaticTherapy_Disease_Indication.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastIndication == null ? 0 : lastIndication.Id;
            itemId = 1;
            if (nonpharmaticTherapy.Indication != null)
            {
                foreach (var item in nonpharmaticTherapy.Indication)
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
                        item.NonpharmaticTherapyId = dbnonpharmaticTherapy.Id;
                        var bond = _mapper.Map<NonpharmaticTherapy_Disease_Indication>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.NonpharmaticTherapy_Disease_Indication.AddAsync(bond);
                    }
                }
            }

            #endregion

            #region medicalFacilities bond
            var lastSideEffect = await _myHeartContext.NonpharmaticTherapy_MedicalFacilities_MedicalIntervention.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastSideEffect == null ? 0 : lastSideEffect.Id;
            itemId = 1;
            if (nonpharmaticTherapy.MedicalIntervention != null)
            {
                foreach (var item in nonpharmaticTherapy.MedicalIntervention)
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
                        item.NonpharmaticTherapyId = dbnonpharmaticTherapy.Id;
                        var bond = _mapper.Map<NonpharmaticTherapy_MedicalFacilities_MedicalIntervention>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.NonpharmaticTherapy_MedicalFacilities_MedicalIntervention.AddAsync(bond);
                    }
                }
            }

            #endregion

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<NonpharmaticTherapy, NonpharmacyDTO>(dbnonpharmaticTherapy);
        }

        public async Task<NonpharmacyDTO> DeleteNonpharmacy(int nonpharmaticTherapyId)
        {
            var dbnonpharmaticTherapy = await _myHeartContext.NonpharmaticTherapy.FirstOrDefaultAsync(u => u.Id == nonpharmaticTherapyId);

            if (dbnonpharmaticTherapy == null)
            {
                return null;
            }

            _myHeartContext.NonpharmaticTherapy.Remove(dbnonpharmaticTherapy);

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<NonpharmacyDTO>(dbnonpharmaticTherapy);
        }


        public async Task<Dictionary<string, string>> ValidateNonpharmacy(NonpharmacyDTO therapyNews)
        {
            var errorList = new Dictionary<string, string>();

            if (therapyNews.Description == null || therapyNews.Description.Length < 2)
                errorList.Add("Popis", "Popis nesmí být kratší než 2 znaky");

            return errorList;
        }

        public async Task<IEnumerable<UserNonpharmacyDto>> GetAllUserNonpharmacy()
        {
            var userDiseaseList = await _myHeartContext.UserNonpharmacy
                .Include(x => x.NonpharmaticTherapy)
                .ToListAsync();

            return userDiseaseList.Select(_mapper.Map<UserNonpharmacyDto>);
        }
    }
}