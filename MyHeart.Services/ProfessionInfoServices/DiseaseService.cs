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
using Org.BouncyCastle.Ocsp;
using MyHeart.DTO.User;
using MyHeart.Data.Migrations;
using MyHeart.Data.Models.ProfessionInformation;

namespace MyHeart.Services
{
    public class DiseaseService : IDiseaseService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MyHeartContext _myHeartContext;
        private readonly IConfiguration _configuration;

        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public DiseaseService(IHttpContextAccessor httpContextAccessor, MyHeartContext gradologyContext, IMapper mapper, IConfiguration configuration, IEmailService emailService, IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _myHeartContext = gradologyContext;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
            _userService = userService;
        }

        public async Task<IEnumerable<DiseaseDTO>> DiseaseListAsync()
        {
            var disease = await _myHeartContext.Disease.ToListAsync();

            return _mapper.Map<IEnumerable<Disease>, IEnumerable<DiseaseDTO>>(disease);
        }

        public async Task<DataTableResponse<DiseaseDTO>> GetDiseasesTable(DataTableRequest request)
        {
            List<Disease> diseases;
            int totalCount;

            //no need to search all strings if there is no filter
            if (!string.IsNullOrEmpty(request.Filter))
            {
                //no need to filter twice
                var filtered = _myHeartContext.Disease
                    .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()));

                diseases = await filtered
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                totalCount = await filtered
                    .CountAsync();
            }
            else
            {
                diseases = await _myHeartContext.Disease
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                totalCount = await _myHeartContext.Disease
                    .CountAsync();
            }

            return new DataTableResponse<DiseaseDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<Disease>, IEnumerable<DiseaseDTO>>(diseases).ToList()
            };
        }

        public async Task<DataTableResponse<DiseaseDTO>> GetPagedResource(SortedPagedSourceRequest request)
        {
            List<Disease> diseases;
            int totalCount;
            var selected = request.Selected.ToList();
            var clientPages = (int)Math.Ceiling((double)selected.Count / request.PageSize);
            var firstPageOffset = selected.Count % request.PageSize;

            var dis = _myHeartContext.Disease.AsQueryable();

            //no need to search all strings if there is no filter
            if (!string.IsNullOrEmpty(request.Filter))
            {
                //no need to filter twice
                dis = dis
                    .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()));

                diseases = await dis
                        .Skip((request.Page - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync();

                totalCount = await dis.CountAsync();
            }
            else
            {
                if (request.Selected.Count() > 0)
                {
                    dis = dis
                        .Where(x => !selected.Contains(x.Id));
                }

                if (request.Page < clientPages || (request.Page == clientPages && firstPageOffset == 0))
                {
                    diseases = new List<Disease>();
                }
                else if (request.Page == clientPages)
                {
                    diseases = await dis
                        .Take(request.PageSize - firstPageOffset)
                        .ToListAsync();
                }
                else
                {
                    diseases = await dis
                        .Skip((request.Page - clientPages - 1) * request.PageSize + (clientPages == 0 ? 0 : (request.PageSize - firstPageOffset)))
                        .Take(request.PageSize)
                        .ToListAsync();
                }

                totalCount = (await dis.CountAsync()) + selected.Count;
            }

            return new DataTableResponse<DiseaseDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<Disease>, IEnumerable<DiseaseDTO>>(diseases).ToList()
            };
        }

        public async Task<IEnumerable<DiseaseDTO>> GetByIds(IEnumerable<int> ids)
        {
            var disease = await _myHeartContext.Disease
                .Where(x => ids.Contains(x.Id)).ToListAsync();

            return _mapper.Map<IEnumerable<Disease>, IEnumerable<DiseaseDTO>>(disease);
        }

        public async Task<DiseaseDTO> DiseaseAsync(int diseaseId)
        {
            var disease = await _myHeartContext.Disease
                .Where(d => d.Id == diseaseId)
                .Include(x => x.Symptoms)
                .Include(p => p.Causes)
                .Include(p => p.MedicamentGroup)
                .Include(p => p.NonpharmaticTherapy)
                .Include(p => p.PreventiveMeasures)
                .Include(x => x.Synonyms)
                .FirstOrDefaultAsync();

            return _mapper.Map<Disease, DiseaseDTO>(disease);
        }

        public async Task<DiseaseDTO> UpdateDisease(DiseaseDTO disease, string user)
        {
            //var dbdisease = await _myHeartContext.Disease.FirstOrDefaultAsync(u => u.Id == disease.Id);
            var dbDisease = await _myHeartContext.Disease
                .Where(d => d.Id == disease.Id)
                .Include(x => x.Symptoms)
                .Include(p => p.Causes)
                .Include(p => p.MedicamentGroup)
                .Include(p => p.NonpharmaticTherapy)
                .Include(x => x.Synonyms)
                .Include(p => p.PreventiveMeasures)
                .FirstOrDefaultAsync();

            if (dbDisease == null)
            {
                return null;
            }

            dbDisease.Name = disease.Name;
            dbDisease.Description = disease.Description;

            int? parsedFrequency = null;
            if (!string.IsNullOrEmpty(disease.Frequency))
            {
                int.TryParse(disease.Frequency, out int freq);
                parsedFrequency = freq;
            }

            dbDisease.Frequency = parsedFrequency;
            dbDisease.TargetWeight = disease.TargetWeight;
            dbDisease.TargetSystolicPressure = disease.TargetSystolicPressure;
            dbDisease.TargetDiastolicPressure = disease.TargetDiastolicPressure;
            dbDisease.TargetHeartRate = disease.TargetHeartRate;
            dbDisease.TargetLdl = disease.TargetLdl;
            dbDisease.SystemicMeasures = disease.SystemicMeasures;
            dbDisease.LastUpdateUser = user;
            //update causes bond
            #region update Causes
            int bondCount = dbDisease.Causes.Count;
            List<int> idList = new List<int>();
            List<Disease_Disease_Causes> causesList = new List<Disease_Disease_Causes>();
            var lastCause = await _myHeartContext.Disease_Disease_Causes.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastCause == null ? 0 : lastCause.Id;
            int itemId = 1;
            foreach (var item in disease.Causes)
            {
                if (item.Id == 0)
                {
                    if (lastCause != null)
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
                    foreach (var existItem in dbDisease.Causes)
                    {
                        if (existItem.CauseId == item.CauseId && existItem.DiseaseId == item.DiseaseId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        causesList.Add(_mapper.Map<Disease_Disease_Causes>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<Disease_Disease_Causes>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.Disease_Disease_Causes.AddAsync(bond);
                        idList.Add(item.Id);
                    }

                }
                else
                {
                    causesList.Add(_mapper.Map<Disease_Disease_Causes>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbDisease.Causes)
                {
                    foreach (var cause in causesList)
                    {
                        if (cause.CauseId == item.CauseId && cause.DiseaseId == item.DiseaseId)
                        {
                            //item.Id = symptom.Id;
                            item.BondStrength = cause.BondStrength;
                            item.LastUpdateUser = user;
                        }

                    }
                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.Disease_Disease_Causes.Remove(item);
                    }
                }
            }
            #endregion

            //update symtopms bond
            #region update Symptoms
            bondCount = dbDisease.Symptoms.Count;
            idList = new List<int>();
            List<Disease_Symptoms_Symptoms> symptomList = new List<Disease_Symptoms_Symptoms>();
            var lastSymptom = await _myHeartContext.Disease_Symptoms_Symptoms.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastSymptom == null ? 0 : lastSymptom.Id;
            itemId = 1;
            foreach (var item in disease.Symptoms)
            {
                if (item.Id == 0)
                {
                    if (lastSymptom != null)
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
                    foreach (var existItem in dbDisease.Symptoms)
                    {
                        if (existItem.SymptomsId == item.SymptomsId && existItem.DiseaseId == item.DiseaseId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        symptomList.Add(_mapper.Map<Disease_Symptoms_Symptoms>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<Disease_Symptoms_Symptoms>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.Disease_Symptoms_Symptoms.AddAsync(bond);
                        idList.Add(item.Id);
                    }


                }
                else
                {
                    symptomList.Add(_mapper.Map<Disease_Symptoms_Symptoms>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbDisease.Symptoms)
                {
                    foreach (var symptom in symptomList)
                    {
                        if (symptom.SymptomsId == item.SymptomsId && symptom.DiseaseId == item.DiseaseId)
                        {
                            //item.Id = symptom.Id;
                            item.BondStrength = symptom.BondStrength;
                            item.LastUpdateUser = user;
                        }

                    }
                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.Disease_Symptoms_Symptoms.Remove(item);
                    }
                }
            }
            #endregion

            //update medicamentGroup bond
            #region update MedicamentGroup
            bondCount = dbDisease.MedicamentGroup.Count;
            List<MedicamentGroup_Disease_Indication> mediList = new List<MedicamentGroup_Disease_Indication>();
            idList = new List<int>();
            var lastMedicamentGroup = await _myHeartContext.MedicamentGroup_Disease_Indication.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastMedicamentGroup == null ? 0 : lastMedicamentGroup.Id;
            itemId = 1;
            foreach (var item in disease.MedicamentGroup)
            {
                if (item.Id == 0)
                {
                    if (lastMedicamentGroup != null)
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
                    foreach (var existItem in dbDisease.MedicamentGroup)
                    {
                        if (existItem.MedicamentGroupId == item.MedicamentGroupId && existItem.DiseaseId == item.DiseaseId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        mediList.Add(_mapper.Map<MedicamentGroup_Disease_Indication>(item));
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
                    mediList.Add(_mapper.Map<MedicamentGroup_Disease_Indication>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbDisease.MedicamentGroup)
                {
                    foreach (var medicament in mediList)
                    {
                        if (medicament.MedicamentGroupId == item.MedicamentGroupId && medicament.DiseaseId == item.DiseaseId)
                        {
                            //item.Id = symptom.Id;
                            item.BondStrength = medicament.BondStrength;
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

            //update preventive bond
            #region update preventive
            bondCount = dbDisease.PreventiveMeasures.Count;
            idList = new List<int>();
            List<Disease_PreventiveMeasures_PreventiveMeasures> preveList = new List<Disease_PreventiveMeasures_PreventiveMeasures>();

            var lastPreventive = await _myHeartContext.Disease_PreventiveMeasures_PreventiveMeasures.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastPreventive == null ? 0 : lastPreventive.Id;
            itemId = 1;
            foreach (var item in disease.PreventiveMeasures)
            {
                if (item.Id == 0)
                {
                    if (lastMedicamentGroup != null)
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
                    foreach (var existItem in dbDisease.PreventiveMeasures)
                    {
                        if (existItem.PreventiveMeasuresId == item.PreventiveMeasuresId && existItem.DiseaseId == item.DiseaseId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        preveList.Add(_mapper.Map<Disease_PreventiveMeasures_PreventiveMeasures>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<Disease_PreventiveMeasures_PreventiveMeasures>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.Disease_PreventiveMeasures_PreventiveMeasures.AddAsync(bond);
                        idList.Add(item.Id);
                    }

                }
                else
                {
                    preveList.Add(_mapper.Map<Disease_PreventiveMeasures_PreventiveMeasures>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbDisease.PreventiveMeasures)
                {
                    foreach (var preventive in preveList)
                    {
                        if (preventive.PreventiveMeasuresId == item.PreventiveMeasuresId && preventive.DiseaseId == item.DiseaseId)
                        {
                            //item.Id = symptom.Id;
                            item.BondStrength = preventive.BondStrength;
                            item.LastUpdateUser = user;
                        }

                    }

                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.Disease_PreventiveMeasures_PreventiveMeasures.Remove(item);
                    }
                }
            }
            #endregion

            //update nonpharmacy bond
            #region update nonpharmacy
            bondCount = dbDisease.NonpharmaticTherapy.Count;
            List<Disease_NonpharmaticTherapy_NonpharmaticTherapy> nonpharmaList = new List<Disease_NonpharmaticTherapy_NonpharmaticTherapy>();
            idList = new List<int>();
            var lastNonpharmacy = await _myHeartContext.Disease_NonpharmaticTherapy_NonpharmaticTherapy.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastNonpharmacy == null ? 0 : lastNonpharmacy.Id;
            itemId = 1;
            foreach (var item in disease.NonpharmaticTherapy)
            {
                if (item.Id == 0)
                {
                    if (lastNonpharmacy != null)
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
                    foreach (var existItem in dbDisease.NonpharmaticTherapy)
                    {
                        if (existItem.NonpharmaticTherapyId == item.NonpharmaticTherapyId && existItem.DiseaseId == item.DiseaseId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        nonpharmaList.Add(_mapper.Map<Disease_NonpharmaticTherapy_NonpharmaticTherapy>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<Disease_NonpharmaticTherapy_NonpharmaticTherapy>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.Disease_NonpharmaticTherapy_NonpharmaticTherapy.AddAsync(bond);
                        idList.Add(item.Id);
                    }

                }
                else
                {
                    nonpharmaList.Add(_mapper.Map<Disease_NonpharmaticTherapy_NonpharmaticTherapy>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbDisease.NonpharmaticTherapy)
                {
                    foreach (var nonpharma in nonpharmaList)
                    {
                        if (nonpharma.NonpharmaticTherapyId == item.NonpharmaticTherapyId && nonpharma.DiseaseId == item.DiseaseId)
                        {
                            //item.Id = symptom.Id;
                            item.BondStrength = nonpharma.BondStrength;
                            item.LastUpdateUser = user;
                        }

                    }

                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.Disease_NonpharmaticTherapy_NonpharmaticTherapy.Remove(item);
                    }
                }
            }
            #endregion

            //update synonyms
            #region update synonyms

            foreach (var dbSynonym in dbDisease.Synonyms)
            {
                var exists = disease.Synonyms.Any(x => x.Id == dbSynonym.Id);
                if (!exists)
                {
                    _myHeartContext.Remove(dbSynonym);
                }
            }

            foreach (var synonym in disease.Synonyms)
            {
                if (synonym.Id == 0)
                {
                    var dbSynonym = _mapper.Map<Disease_Synonyms>(synonym);
                    dbSynonym.DiseaseId = disease.Id;
                    _myHeartContext.Add(dbSynonym);
                }
                else
                {
                    var existingSynonym = dbDisease.Synonyms.FirstOrDefault(x => x.Id == synonym.Id);
                    if (existingSynonym != null)
                    {
                        existingSynonym.Name = synonym.Name;
                    }
                }
            }



            #endregion

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<DiseaseDTO>(disease);
        }



        public async Task<DiseaseDTO> AddDiseaseAsync(DiseaseDTO disease, string user)
        {
            int? parsedFrequency = null;
            if (!string.IsNullOrEmpty(disease.Frequency))
            {
                int.TryParse(disease.Frequency, out int freq);
                parsedFrequency = freq;
            }


            var dbDisease = new Disease()
            {
                Name = disease.Name,
                Description = disease.Description,
                Frequency = parsedFrequency,
                LastUpdateUser = user,
                TargetDiastolicPressure = disease.TargetDiastolicPressure,
                TargetHeartRate = disease.TargetHeartRate,
                TargetLdl = disease.TargetLdl,
                TargetSystolicPressure = disease.TargetSystolicPressure,
                TargetWeight = disease.TargetWeight,
                SystemicMeasures = disease.SystemicMeasures,
                Synonyms = _mapper.Map<List<Disease_Synonyms>>(disease.Synonyms)
            };

            _myHeartContext.Add(dbDisease);

            await _myHeartContext.SaveChangesAsync();

            #region update Causes
            var lastCause = await _myHeartContext.Disease_Disease_Causes.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastCause == null ? 0 : lastCause.Id;
            int itemId = 1;
            if (disease.Causes != null)
            {
                foreach (var item in disease.Causes)
                {
                    if (item.Id == 0)
                    {
                        if (lastCause != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.DiseaseId = dbDisease.Id;
                        var bond = _mapper.Map<Disease_Disease_Causes>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.Disease_Disease_Causes.AddAsync(bond);
                    }
                }
            }

            #endregion

            #region update Symptoms
            var lastSymptom = await _myHeartContext.Disease_Symptoms_Symptoms.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastSymptom == null ? 0 : lastSymptom.Id;
            itemId = 1;

            if (disease.Symptoms != null)
            {
                foreach (var item in disease.Symptoms)
                {
                    if (item.Id == 0)
                    {
                        if (lastSymptom != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.DiseaseId = dbDisease.Id;
                        var bond = _mapper.Map<Disease_Symptoms_Symptoms>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.Disease_Symptoms_Symptoms.AddAsync(bond);
                    }
                }
            }

            #endregion

            #region update MedicamentGroup
            var lastMedicamentGroup = await _myHeartContext.MedicamentGroup_Disease_Indication.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastMedicamentGroup == null ? 0 : lastMedicamentGroup.Id;
            itemId = 1;
            if (disease.MedicamentGroup != null)
            {
                foreach (var item in disease.MedicamentGroup)
                {
                    if (item.Id == 0)
                    {
                        if (lastMedicamentGroup != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.DiseaseId = dbDisease.Id;
                        var bond = _mapper.Map<MedicamentGroup_Disease_Indication>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.MedicamentGroup_Disease_Indication.AddAsync(bond);
                    }
                }
            }

            #endregion

            #region update preventive
            var lastPreventive = await _myHeartContext.Disease_PreventiveMeasures_PreventiveMeasures.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastPreventive == null ? 0 : lastPreventive.Id;
            itemId = 1;
            if (disease.PreventiveMeasures != null)
            {
                foreach (var item in disease.PreventiveMeasures)
                {
                    if (item.Id == 0)
                    {
                        if (lastMedicamentGroup != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.DiseaseId = dbDisease.Id;
                        var bond = _mapper.Map<Disease_PreventiveMeasures_PreventiveMeasures>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.Disease_PreventiveMeasures_PreventiveMeasures.AddAsync(bond);
                    }
                }
            }

            #endregion

            #region update nonpharmacy
            var lastNonpharmacy = await _myHeartContext.Disease_NonpharmaticTherapy_NonpharmaticTherapy.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastNonpharmacy == null ? 0 : lastNonpharmacy.Id;
            itemId = 1;
            if (disease.NonpharmaticTherapy != null)
            {
                foreach (var item in disease.NonpharmaticTherapy)
                {
                    if (item.Id == 0)
                    {
                        if (lastNonpharmacy != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.DiseaseId = dbDisease.Id;
                        var bond = _mapper.Map<Disease_NonpharmaticTherapy_NonpharmaticTherapy>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.Disease_NonpharmaticTherapy_NonpharmaticTherapy.AddAsync(bond);
                    }
                }
            }

            #endregion
            await _myHeartContext.SaveChangesAsync();
            return _mapper.Map<Disease, DiseaseDTO>(dbDisease);
        }

        public async Task<DiseaseDTO> DeleteDisease(int diseaseId)
        {
            var disease = await _myHeartContext.Disease.FirstOrDefaultAsync(u => u.Id == diseaseId);

            if (disease == null)
            {
                return null;
            }

            _myHeartContext.Disease.Remove(disease);

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<DiseaseDTO>(disease);
        }

        public async Task<IEnumerable<UserDiseaseDto>> GetAllUserDiseases()
        {
            var userDiseaseList = await _myHeartContext.UserDiseases
                .Include(x => x.Disease)
                .ToListAsync();

            return userDiseaseList.Select(_mapper.Map<UserDiseaseDto>);
        }

        #region Validation

        public async Task<Dictionary<string, string>> ValidateDisease(DiseaseDTO therapyNews)
        {
            var errorList = new Dictionary<string, string>();

            if (therapyNews.Name.Length < 2)
                errorList.Add("Text", "Text nesmí být kratší než 2 znaky");

            return errorList;
        }
        #endregion

    }
}