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

namespace MyHeart.Services
{
    public class SymptomService : ISymptomService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MyHeartContext _myHeartContext;
        private readonly IConfiguration _configuration;

        private readonly IEmailService _emailService;

        public SymptomService(IHttpContextAccessor httpContextAccessor, MyHeartContext gradologyContext, IMapper mapper, IConfiguration configuration, IEmailService emailService)
        {
            _httpContextAccessor = httpContextAccessor;
            _myHeartContext = gradologyContext;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<IEnumerable<SymptomDTO>> SymptomsListAsync()
        {
            var symptoms = await _myHeartContext.Symptoms.ToListAsync();

            return _mapper.Map<IEnumerable<Symptoms>, IEnumerable<SymptomDTO>>(symptoms);
        }

        public async Task<DataTableResponse<SymptomDTO>> GetSymptomsTable(DataTableRequest request)
        {
            List<Symptoms> symptoms;
            int totalCount;

            //no need to search all strings if there is no filter
            if (!string.IsNullOrEmpty(request.Filter))
            {
                //no need to filter twice
                var filtered = _myHeartContext.Symptoms
                    .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()));

                symptoms = await filtered
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                totalCount = await filtered
                    .CountAsync();
            }
            else
            {
                symptoms = await _myHeartContext.Symptoms
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                totalCount = await _myHeartContext.Symptoms
                    .CountAsync();
            }

            return new DataTableResponse<SymptomDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<Symptoms>, IEnumerable<SymptomDTO>>(symptoms).ToList()
            };
        }

        public async Task<DataTableResponse<SymptomDTO>> GetPagedResource(SortedPagedSourceRequest request)
        {
            List<Symptoms> symptoms;
            int totalCount;
            var selected = request.Selected.ToList();
            var clientPages = (int)Math.Ceiling((double)selected.Count / request.PageSize);
            var firstPageOffset = selected.Count % request.PageSize;

            var symp = _myHeartContext.Symptoms.AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter))
            {
                symp = symp
                    .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()));

                symptoms = await symp
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                totalCount = await symp.CountAsync();
            }
            else
            {
                if (request.Selected.Count() > 0)
                {
                    symp = symp
                        .Where(x => !selected.Contains(x.Id));
                }

                if (request.Page < clientPages || (request.Page == clientPages && firstPageOffset == 0))
                {
                    symptoms = new List<Symptoms>();
                }
                else if (request.Page == clientPages)
                {
                    symptoms = await symp
                        .Take(request.PageSize - firstPageOffset)
                        .ToListAsync();
                }
                else
                {
                    symptoms = await symp
                        .Skip((request.Page - clientPages - 1) * request.PageSize + (clientPages == 0 ? 0 : (request.PageSize - firstPageOffset)))
                        .Take(request.PageSize)
                        .ToListAsync();
                }

                totalCount = (await symp.CountAsync()) + selected.Count;
            }



            return new DataTableResponse<SymptomDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<Symptoms>, IEnumerable<SymptomDTO>>(symptoms).ToList()
            };
        }

        public async Task<IEnumerable<SymptomDTO>> GetByIds(IEnumerable<int> ids)
        {
            var symptoms = await _myHeartContext.Symptoms
                .Where(x => ids.Contains(x.Id)).ToListAsync();

            return _mapper.Map<IEnumerable<Symptoms>, IEnumerable<SymptomDTO>>(symptoms);
        }

        public async Task<SymptomDTO> SymptomAsync(int symptomId)
        {
            var symptom = await _myHeartContext.Symptoms
                .Where(d => d.Id == symptomId)
                .Include(x => x.Diseases)
                .Include(x => x.SymptomQuestions)
                .Include(x => x.Synonyms)
                .FirstOrDefaultAsync();

            return _mapper.Map<Symptoms, SymptomDTO>(symptom);
        }

        public async Task<SymptomDTO> UpdateSymptom(SymptomDTO symptom, string user)
        {
            var dbsymptom = await _myHeartContext.Symptoms
                .Where(d => d.Id == symptom.Id)
                .Include(x => x.Diseases)
                .Include(x => x.SymptomQuestions)
                .Include(x => x.Synonyms)
                .FirstOrDefaultAsync();

            if (dbsymptom == null)
            {
                return null;
            }

            dbsymptom.Name = symptom.Name;
            dbsymptom.Description = symptom.Description;
            dbsymptom.LastUpdateUser = user;

            #region disease bond
            int bondCount = dbsymptom.Diseases.Count;
            List<int> idList = new List<int>();
            List<Disease_Symptoms_Symptoms> diseaseList = new List<Disease_Symptoms_Symptoms>();
            var lastDisease = await _myHeartContext.Disease_Symptoms_Symptoms.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastDisease == null ? 0 : lastDisease.Id;
            int itemId = 1;
            foreach (var item in symptom.Diseases)
            {
                if (item.Id == 0)
                {
                    if (lastDisease != null)
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
                    foreach (var existItem in dbsymptom.Diseases)
                    {
                        if (existItem.SymptomsId == item.SymptomsId && existItem.DiseaseId == item.DiseaseId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        diseaseList.Add(_mapper.Map<Disease_Symptoms_Symptoms>(item));
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
                    diseaseList.Add(_mapper.Map<Disease_Symptoms_Symptoms>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbsymptom.Diseases)
                {
                    foreach (var disease in diseaseList)
                    {
                        if (disease.SymptomsId == item.SymptomsId && disease.DiseaseId == item.DiseaseId)
                        {
                            item.BondStrength = disease.BondStrength;
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

            #region symptom question bond
            int symptombondCount = dbsymptom.Diseases.Count;
            List<int> symptomIdList = new List<int>();
            List<SymptomQuestions_Symptom> symptomList = new List<SymptomQuestions_Symptom>();
            var lastSymptom = await _myHeartContext.SymptomQuestions_Symptom.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastSymptomId = lastSymptom == null ? 0 : lastSymptom.Id;
            int symptomItemId = 1;
            foreach (var item in symptom.SymptomQuestions)
            {
                if (item.Id == 0)
                {
                    if (lastSymptom != null)
                    {
                        lastSymptomId++;
                        item.Id = lastSymptomId;
                    }
                    else
                    {
                        item.Id = symptomItemId;
                        symptomItemId++;
                    }
                    bool exist = false;
                    if (dbsymptom.SymptomQuestions != null)
                    {
                        foreach (var existItem in dbsymptom.SymptomQuestions)
                        {
                            if (existItem.SymptomsId == item.SymptomId && existItem.SymptomQuestionsId == item.SymptomQuestionsId)
                            {
                                symptomIdList.Add(existItem.Id);
                                exist = true;
                            }
                        }
                    }

                    if (exist)
                    {
                        symptomList.Add(_mapper.Map<SymptomQuestions_Symptom>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<SymptomQuestions_Symptom>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.SymptomQuestions_Symptom.AddAsync(bond);
                        symptomIdList.Add(item.Id);
                    }

                }
                else
                {
                    symptomList.Add(_mapper.Map<SymptomQuestions_Symptom>(item));
                    symptomIdList.Add(item.Id);
                }
            }
            if (symptombondCount != 0)
            {
                foreach (var item in dbsymptom.SymptomQuestions)
                {
                    foreach (var symptomQuestion in symptomList)
                    {
                        if (symptomQuestion.SymptomsId == item.SymptomsId && symptomQuestion.SymptomQuestionsId == item.SymptomQuestionsId)
                        {
                            item.BondStrength = symptomQuestion.BondStrength;
                            item.LastUpdateUser = user;
                        }

                    }
                    if (!symptomIdList.Contains(item.Id))
                    {
                        _myHeartContext.SymptomQuestions_Symptom.Remove(item);
                    }
                }
            }

            #endregion

            #region synonyms

            foreach (var synonym in dbsymptom.Synonyms)
            {
                var exists = symptom.Synonyms.Any(x => x.Id == synonym.Id);
                if (!exists)
                {
                    _myHeartContext.Remove(synonym);
                }
            }

            foreach (var synonym in symptom.Synonyms)
            {
                if (synonym.Id == 0)
                {
                    var dbSynonym = _mapper.Map<Symptom_Synonyms>(synonym);
                    _myHeartContext.Add(dbSynonym);
                }
                else
                {
                    var existingSynonym = dbsymptom.Synonyms.FirstOrDefault(x => x.Id == symptom.Id);
                    if (existingSynonym != null)
                    {
                        existingSynonym.Name = symptom.Name;
                    }
                }
            }

            #endregion

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<SymptomDTO>(symptom);
        }

        public async Task<SymptomDTO> AddSymptomAsync(SymptomDTO symptom, string user)
        {
            var dbsymptom = new Symptoms()
            {
                Name = symptom.Name,
                Description = symptom.Description,
                LastUpdateUser = user,
                Synonyms = _mapper.Map<List<Symptom_Synonyms>>(symptom.Synonyms)
            };

            _myHeartContext.Add(dbsymptom);

            await _myHeartContext.SaveChangesAsync();
            #region disease bond
            var lastDisease = await _myHeartContext.Disease_Symptoms_Symptoms.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastDisease == null ? 0 : lastDisease.Id;
            int itemId = 1;
            if (symptom.Diseases != null)
            {
                foreach (var item in symptom.Diseases)
                {
                    if (item.Id == 0)
                    {
                        if (lastDisease != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.SymptomsId = dbsymptom.Id;
                        var bond = _mapper.Map<Disease_Symptoms_Symptoms>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.Disease_Symptoms_Symptoms.AddAsync(bond);
                    }
                }
            }

            #endregion
            await _myHeartContext.SaveChangesAsync();
            return _mapper.Map<Symptoms, SymptomDTO>(dbsymptom);
        }

        public async Task<SymptomDTO> DeleteSymptom(int symptomId)
        {
            var symptom = await _myHeartContext.Symptoms.FirstOrDefaultAsync(u => u.Id == symptomId);

            if (symptom == null)
            {
                return null;
            }

            _myHeartContext.Symptoms.Remove(symptom);

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<SymptomDTO>(symptom);
        }


        public async Task<Dictionary<string, string>> ValidateSymptom(SymptomDTO symptom)
        {
            var errorList = new Dictionary<string, string>();

            if (await _myHeartContext.Symptoms.AnyAsync(u => u.Name == symptom.Name))
                errorList.Add("Název", "Název symptomu již existuje");

            return errorList;
        }
    }
}