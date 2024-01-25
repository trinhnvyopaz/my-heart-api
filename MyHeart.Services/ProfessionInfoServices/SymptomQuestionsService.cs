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
using OfficeOpenXml.ConditionalFormatting;
using MyHeart.DTO.ProfessionInformation;
using System;

namespace MyHeart.Services
{
    public class SymptomQuestionsService : ISymptomQuestionsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MyHeartContext _myHeartContext;
        private readonly IConfiguration _configuration;

        private readonly IEmailService _emailService;

        public SymptomQuestionsService(IHttpContextAccessor httpContextAccessor, MyHeartContext gradologyContext, IMapper mapper, IConfiguration configuration, IEmailService emailService)
        {
            _httpContextAccessor = httpContextAccessor;
            _myHeartContext = gradologyContext;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<IEnumerable<SymptomQuestionsDTO>> SymptomQuestionsListAsync()
        {
            var symptomQuestions = await _myHeartContext.SymptomQuestions.ToListAsync();

            return _mapper.Map<IEnumerable<SymptomQuestions>, IEnumerable<SymptomQuestionsDTO>>(symptomQuestions);
        }

        public async Task<DataTableResponse<SymptomQuestionsDTO>> GetSymptomQuestionsTable(DataTableRequest request)
        {
            List<SymptomQuestions> questions;
            int totalCount;

            //questions might be really long, no need to work with strings when filter is empty
            if (!string.IsNullOrEmpty(request.Filter))
            {
                //we need to get diseases and symptoms that might match the filter
                var diseaseIds = await _myHeartContext.Disease
                    .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()))
                    .Select(x => x.Id)
                    .ToArrayAsync();

                var symptomIds = await _myHeartContext.Symptoms
                    .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()))
                    .Select(x => x.Id)
                    .ToArrayAsync();

                //no need to filter twice
                var filtered = _myHeartContext.SymptomQuestions
                    .Where(x => x.Text.ToLower().Contains(request.Filter.ToLower())
                        || x.Diseases.Any(y => diseaseIds.Any(z => y.DiseaseId == z))
                        || x.Symptoms.Any(y => symptomIds.Any(z => y.SymptomsId == z)));

                questions = await filtered
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Include(x => x.Diseases)
                    .Include(x => x.Symptoms)
                    .ToListAsync();

                totalCount = await filtered
                    .CountAsync();
            }
            else
            {
                questions = await _myHeartContext.SymptomQuestions
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Include(x => x.Diseases)
                    .Include(x => x.Symptoms)
                    .ToListAsync();

                totalCount = await _myHeartContext.SymptomQuestions
                    .CountAsync();
            }

            var questionDTOs = _mapper.Map<IEnumerable<SymptomQuestions>, IEnumerable<SymptomQuestionsDTO>>(questions).ToList();

            foreach (var question in questionDTOs)
            {
                if (question.Symptoms.Count > 0)
                {
                    var symptomIds = question.Symptoms.Select(x => x.SymptomId).ToArray();
                    var symptoms = await _myHeartContext.Symptoms
                        .Where(x => symptomIds.Contains(x.Id))
                        .Select(x => x.Name)
                        .ToListAsync();
                    question.SymptomName = string.Join(", ", symptoms);
                }

                if (question.Diseases.Count > 0)
                {
                    var diseaseIds = question.Diseases.Select(x => x.DiseaseId).ToArray();
                    var diseases = await _myHeartContext.Disease
                        .Where(x => diseaseIds.Contains(x.Id))
                        .Select(x => x.Name)
                        .ToListAsync();

                    question.DiseaseString = string.Join(", ", diseases);
                }
            }

            return new DataTableResponse<SymptomQuestionsDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = questionDTOs
            };
        }

        public async Task<SymptomQuestionsDTO> SymptomQuestionsAsync(int symptomQuestionsId)
        {
            var symptomQuestions = await _myHeartContext.SymptomQuestions
                .Where(d => d.Id == symptomQuestionsId)
                .Include(x => x.Symptoms)
                .Include(x => x.Diseases)
                .FirstOrDefaultAsync();

            return _mapper.Map<SymptomQuestions, SymptomQuestionsDTO>(symptomQuestions);
        }

        public async Task<IEnumerable<SymptomQuestionsDTO>> SymptomQuestionsListByDiseaseIdAsync(int diseaseId)
        {
            var symptomQuestions = await _myHeartContext.SymptomQuestions
                .Where(x => x.Diseases.Any(y => y.DiseaseId == diseaseId))
                .Include(x => x.Symptoms)
                .Include(x => x.Diseases)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SymptomQuestions>, IEnumerable<SymptomQuestionsDTO>>(symptomQuestions);
        }

        public async Task<SymptomQuestionsDTO> UpdateSymptomQuestions(SymptomQuestionsDTO symptomQuestions, string user)
        {

            var dbSymptomQuestions = await _myHeartContext.SymptomQuestions
                .Where(d => d.Id == symptomQuestions.Id)
                .Include(x => x.Symptoms)
                .Include(x => x.Diseases)
                .FirstOrDefaultAsync();

            if (dbSymptomQuestions == null)
            {
                return null;
            }

            dbSymptomQuestions.Text = symptomQuestions.Text;
            dbSymptomQuestions.LastUpdateUser = user;

            //update symtopms bond
            #region update Symptoms
            //var symptom = _mapper.Map<SymptomQuestions_Symptom>(symptomQuestions.Symptoms);
            //if (dbSymptomQuestions.Symptoms != symptom) {
            //    symptom.LastUpdateUser = user;
            //    dbSymptomQuestions.Symptoms = symptom;
            //}
            int bondSymptomCount = dbSymptomQuestions.Symptoms.Count;
            var symptomList = new List<SymptomQuestions_Symptom>();
            var symptomIdList = new List<int>();
            var lastSymptom = await _myHeartContext.SymptomQuestions_Symptom.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            var lastSymptomId = lastSymptom == null ? 0 : lastSymptom.Id;
            var symptomId = 1;

            foreach (var item in symptomQuestions.Symptoms)
            {
                if (item.Id == 0)
                {
                    if (lastSymptom != null)
                    {
                        item.Id = ++lastSymptomId;
                    }
                    else
                    {
                        item.Id = symptomId++;
                    }

                    var exists = false;
                    foreach (var existsItem in dbSymptomQuestions.Symptoms)
                    {
                        if (existsItem.SymptomsId == item.SymptomId && existsItem.SymptomQuestionsId == item.SymptomQuestionsId)
                        {
                            symptomIdList.Add(existsItem.Id);
                            exists = true;
                        }
                    }

                    if (exists)
                    {
                        symptomList.Add(_mapper.Map<SymptomQuestions_Symptom>(item));
                    }
                    else
                    {
                        using (var transaction = _myHeartContext.Database.BeginTransaction())
                        {
                            var toSave = _mapper.Map<SymptomQuestions_Symptom>(item);
                            toSave.LastUpdateUser = user;

                            await _myHeartContext.SymptomQuestions_Symptom.AddAsync(toSave);
                            //_myHeartContext.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT dbo.SymptomQuestions_Disease ON;");
                            _myHeartContext.SaveChanges();
                            //_myHeartContext.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT dbo.SymptomQuestions_Disease OFF");
                            transaction.Commit();
                        }

                        symptomIdList.Add(item.Id);
                    }
                }
                else
                {
                    symptomList.Add(_mapper.Map<SymptomQuestions_Symptom>(item));
                    symptomIdList.Add(item.Id);
                }
            }
            if (bondSymptomCount != 0)
            {
                foreach (var item in dbSymptomQuestions.Symptoms)
                {
                    foreach (var symptom in symptomList)
                    {
                        if (symptom.SymptomsId == item.SymptomsId && symptom.SymptomQuestionsId == item.SymptomQuestionsId)
                        {
                            item.BondStrength = symptom.BondStrength;
                            item.LastUpdateUser = user;
                        }
                    }
                    if (symptomIdList.Count > 0 && !symptomIdList.Contains(item.Id))
                    {
                        _myHeartContext.SymptomQuestions_Symptom.Remove(item);
                    }
                }
            }
            #endregion

            //update diseases
            #region update Diseases
            int bondCount = dbSymptomQuestions.Diseases.Count;
            var diseaseList = new List<SymptomQuestions_Disease>();
            var idList = new List<int>();
            var lastDisease = await _myHeartContext.SymptomQuestions_Disease.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            var lastId = lastDisease == null ? 0 : lastDisease.Id;
            var itemId = 1;

            foreach (var item in symptomQuestions.Diseases)
            {
                if (item.Id == 0)
                {
                    if (lastDisease != null)
                    {
                        item.Id = ++lastId;
                    }
                    else
                    {
                        item.Id = itemId++;
                    }

                    var exists = false;
                    foreach (var existsItem in dbSymptomQuestions.Diseases)
                    {
                        if (existsItem.DiseaseId == item.DiseaseId && existsItem.SymptomsQuestionsId == item.SymptomQuestionsId)
                        {
                            idList.Add(existsItem.Id);
                            exists = true;
                        }
                    }

                    if (exists)
                    {
                        diseaseList.Add(_mapper.Map<SymptomQuestions_Disease>(item));
                    }
                    else
                    {
                        using (var transaction = _myHeartContext.Database.BeginTransaction())
                        {
                            var toSave = _mapper.Map<SymptomQuestions_Disease>(item);
                            toSave.LastUpdateUser = user;

                            await _myHeartContext.SymptomQuestions_Disease.AddAsync(toSave);
                            //_myHeartContext.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT dbo.SymptomQuestions_Disease ON;");
                            _myHeartContext.SaveChanges();
                            //_myHeartContext.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT dbo.SymptomQuestions_Disease OFF");
                            transaction.Commit();
                        }

                        idList.Add(item.Id);
                    }
                }
                else
                {
                    diseaseList.Add(_mapper.Map<SymptomQuestions_Disease>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbSymptomQuestions.Diseases)
                {
                    foreach (var disease in diseaseList)
                    {
                        if (disease.DiseaseId == item.DiseaseId && disease.SymptomsQuestionsId == item.SymptomsQuestionsId)
                        {
                            item.BondStrength = disease.BondStrength;
                            item.LastUpdateUser = user;
                        }
                    }
                    if (idList.Count > 0 && !idList.Contains(item.Id))
                    {
                        _myHeartContext.SymptomQuestions_Disease.Remove(item);
                    }
                }
            }
            #endregion

            //add bonds between symptoms and diseases if needed
            #region DiseaseSymptomBond
            foreach (var disease in symptomQuestions.Diseases)
            {
                foreach (var item in symptomQuestions.Symptoms)
                {
                    if (!(await _myHeartContext.Disease_Symptoms_Symptoms.Where(x => x.DiseaseId == disease.DiseaseId && x.SymptomsId == item.SymptomId).AnyAsync()))
                    {
                        var bond = new Disease_Symptoms_Symptoms()
                        {
                            DiseaseId = disease.DiseaseId,
                            SymptomsId = item.SymptomId,
                            BondStrength = disease.BondStrength,
                            LastUpdateUser = user
                        };

                        _myHeartContext.Disease_Symptoms_Symptoms.Add(bond);
                    }
                }

            }
            await _myHeartContext.SaveChangesAsync();
            #endregion

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<SymptomQuestionsDTO>(symptomQuestions);
        }



        public async Task<SymptomQuestionsDTO> AddSymptomQuestionsAsync(SymptomQuestionsDTO symptomQuestions, string user)
        {

            var dbSymptomQuestions = new SymptomQuestions()
            {
                Text = symptomQuestions.Text,
                LastUpdateUser = user
            };

            _myHeartContext.Add(dbSymptomQuestions);

            await _myHeartContext.SaveChangesAsync();

            //update symptom bond
            #region update Symptom
            var lastSymptom = await _myHeartContext.SymptomQuestions_Symptom.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            var lastSymptomId = lastSymptom == null ? 0 : lastSymptom.Id;
            var symptomId = 1;

            if (symptomQuestions.Symptoms != null)
            {
                foreach (var item in symptomQuestions.Symptoms)
                {
                    if (item.Id == 0)
                    {
                        if (lastSymptom != null)
                        {
                            item.Id = ++lastSymptomId;
                        }
                        else
                        {
                            item.Id = symptomId++;
                        }

                        item.SymptomQuestionsId = dbSymptomQuestions.Id;

                        using (var transaction = _myHeartContext.Database.BeginTransaction())
                        {
                            var toSave = _mapper.Map<SymptomQuestions_Symptom>(item);
                            toSave.LastUpdateUser = user;

                            await _myHeartContext.SymptomQuestions_Symptom.AddAsync(toSave);
                            _myHeartContext.SaveChanges();
                            transaction.Commit();
                        }
                    }
                }
            }

            #endregion

            //update diseases
            #region update Diseases
            var lastDisease = await _myHeartContext.SymptomQuestions_Disease.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            var lastId = lastDisease == null ? 0 : lastDisease.Id;
            var itemId = 1;

            if (symptomQuestions.Diseases != null)
            {
                foreach (var item in symptomQuestions.Diseases)
                {
                    if (item.Id == 0)
                    {
                        if (lastDisease != null)
                        {
                            item.Id = ++lastId;
                        }
                        else
                        {
                            item.Id = itemId++;
                        }
                        item.SymptomQuestionsId = dbSymptomQuestions.Id;
                        using (var transaction = _myHeartContext.Database.BeginTransaction())
                        {
                            var toSave = _mapper.Map<SymptomQuestions_Disease>(item);
                            toSave.LastUpdateUser = user;

                            await _myHeartContext.SymptomQuestions_Disease.AddAsync(toSave);
                            _myHeartContext.SaveChanges();
                            transaction.Commit();
                        }

                    }
                }
            }
            #endregion

            //add bonds between symptoms and diseases if needed
            #region DiseaseSymptomBond
            var symptom = symptomQuestions.Symptoms;
            if (symptomQuestions.Diseases != null)
            {
                foreach (var disease in symptomQuestions.Diseases)
                {
                    if (symptom != null)
                    {
                        foreach (var item in symptom)
                        {
                            if (!(await _myHeartContext.Disease_Symptoms_Symptoms.Where(x => x.DiseaseId == disease.DiseaseId && x.SymptomsId == item.SymptomId).AnyAsync()))
                            {
                                var bond = new Disease_Symptoms_Symptoms()
                                {
                                    DiseaseId = disease.DiseaseId,
                                    SymptomsId = item.SymptomId,
                                    BondStrength = disease.BondStrength,
                                    LastUpdateUser = user
                                };

                                _myHeartContext.Disease_Symptoms_Symptoms.Add(bond);
                            }
                        }
                    }

                }
            }
            #endregion

            await _myHeartContext.SaveChangesAsync();
            return _mapper.Map<SymptomQuestions, SymptomQuestionsDTO>(dbSymptomQuestions);
        }

        public async Task<SymptomQuestionsDTO> DeleteSymptomQuestions(int symptomQuestionsId)
        {
            var symptomQuestions = await _myHeartContext.SymptomQuestions.FirstOrDefaultAsync(u => u.Id == symptomQuestionsId);

            if (symptomQuestions == null)
            {
                return null;
            }

            _myHeartContext.SymptomQuestions.Remove(symptomQuestions);

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<SymptomQuestionsDTO>(symptomQuestions);
        }

        public async Task<IEnumerable<SymptomQuestionsDTO>> GetSymptomQuestionsBySymptomIds(IEnumerable<int> symptomIds)
        {
            var symptomQuestions = await _myHeartContext.SymptomQuestions_Symptom
                .Include(s => s.DiseaseQuestions)
                    .ThenInclude(dq => dq.Diseases)
                        .ThenInclude(d => d.Disease)
                .Where(s => symptomIds.Contains(s.SymptomsId))
                .Select(s => s.DiseaseQuestions)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SymptomQuestionsDTO>>(symptomQuestions);
        }
        public async Task<IEnumerable<SymptomQuestions_DiseaseDTO>> UpdateSymptomQuestionsDiseases(IEnumerable<SymptomQuestions_DiseaseDTO> questionDiseases, string user)
        {
            var updatedSymptomDiseases = new List<SymptomQuestions_Disease>();

            foreach (var questionDisease in questionDiseases)
            {
                var dbquestionDisease = await _myHeartContext.SymptomQuestions_Disease.FirstOrDefaultAsync(d => d.Id == questionDisease.Id);

                if (dbquestionDisease != null)
                {
                    dbquestionDisease.BondStrength = questionDisease.BondStrength;
                    dbquestionDisease.LastUpdateUser = user;
                    dbquestionDisease.LastUpdateDate = DateTime.Now;

                    updatedSymptomDiseases.Add(dbquestionDisease);
                }
            }

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<IEnumerable<SymptomQuestions_DiseaseDTO>>(updatedSymptomDiseases);

        }

        public async Task<IEnumerable<SymptomQuestionsDTO>> GetSymptomQuestionsByIds(IEnumerable<int> ids)
        {
            var quetions = await _myHeartContext.SymptomQuestions
                .Where(x => ids.Contains(x.Id)).ToListAsync();

            return _mapper.Map<IEnumerable<SymptomQuestions>, IEnumerable<SymptomQuestionsDTO>>(quetions);
        }

        public async Task<DataTableResponse<SymptomQuestionsDTO>> GetPagedResource(SortedPagedSourceRequest request)
        {
            List<SymptomQuestions> questions;
            int totalCount;
            var selected = request.Selected.ToList();
            var clientPages = (int)Math.Ceiling((double)selected.Count / request.PageSize);
            var firstPageOffset = selected.Count % request.PageSize;

            var ques = _myHeartContext.SymptomQuestions.AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter))
            {
                ques = ques.Where(q => q.Text.ToLower().Contains(request.Filter.ToLower()));

                questions = await ques
                      .Skip((request.Page - 1) * request.PageSize)
                      .Take(request.PageSize)
                      .ToListAsync();

                totalCount = await ques.CountAsync();
            }
            else
            {
                if (request.Selected.Count() > 0)
                {
                    ques = ques.Where(x => !selected.Contains(x.Id));
                }

                if (request.Page < clientPages || (request.Page == clientPages && firstPageOffset == 0))
                {
                    questions = new List<SymptomQuestions>();
                }
                else if (request.Page == clientPages)
                {
                    questions = await ques
                        .Take(request.PageSize - firstPageOffset)
                        .ToListAsync();
                }
                else
                {
                    questions = await ques
                        .Skip((request.Page - clientPages - 1) * request.PageSize + (clientPages == 0 ? 0 : (request.PageSize - firstPageOffset)))
                        .Take(request.PageSize)
                        .ToListAsync();
                }

                totalCount = (await ques.CountAsync()) + selected.Count;
            }

            return new DataTableResponse<SymptomQuestionsDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<SymptomQuestions>, IEnumerable<SymptomQuestionsDTO>>(questions).ToList()
            };

        }

        #region Validation

        public async Task<Dictionary<string, string>> ValidateSymptomQuestions(SymptomQuestionsDTO symptomQuestions)
        {
            var errorList = new Dictionary<string, string>();

            if (symptomQuestions.Text == null || symptomQuestions.Text.Length < 2)
                errorList.Add("Text", "Text nesmí být kratší než 2 znaky");

            return errorList;
        }

        #endregion

    }
}