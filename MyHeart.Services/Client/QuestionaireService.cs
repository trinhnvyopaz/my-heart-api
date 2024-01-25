using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.DTO;
using MyHeart.DTO.Client;
using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO.ProfessionInformation;
using MyHeart.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services
{
    public class QuestionaireService : IQuestionaireService
    {
        private MyHeartContext _myheartContext;
        private IMapper _mapper;
        private IUserService _userService;
        const int probableThreshold = 7;
        const int possibleThreshold = 3;

        public QuestionaireService(MyHeartContext context, IMapper mapper, IUserService userService)
        {
            _myheartContext = context;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<Client_QuestionaireDTO> CreateQuestionaire(Client_QuestionaireDTO questionaire)
        {
            var dbQuestionaire = _mapper.Map<Client_Questionaire>(questionaire);

            var user = await _userService.CurrentUserAsync();

            dbQuestionaire.UserId = user.Id;
            dbQuestionaire.CreatedDate = DateTime.Now;

            _myheartContext.Add(dbQuestionaire);

            await _myheartContext.SaveChangesAsync();

            return _mapper.Map<Client_QuestionaireDTO>(dbQuestionaire);
        }

        public async Task<DataTableResponse<Client_QuestionaireDTO>> GetQuestionaireTable(DataTableRequest request)
        {
            List<Client_Questionaire> questionaires;
            int totalCount = 0;

            if (!string.IsNullOrEmpty(request.Filter))
            {
                var filtered = _myheartContext.Client_Questionaire
                    .Where(x => x.User.FirstName.ToLower().Contains(request.Filter.ToLower()))
                    .Where(x => x.User.FirstName.ToLower().Contains(request.Filter.ToLower()));

                questionaires = await filtered
                    .Include(x => x.User)
                    .ApplyOrdering(request.ColumnOrders)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                totalCount = await filtered
                    .CountAsync();
            }
            else
            {
                questionaires = await _myheartContext.Client_Questionaire
                 .Include(q => q.User)
                 .ApplyOrdering(request.ColumnOrders)
                 .Skip((request.Page - 1) * request.PageSize)
                 .Take(request.PageSize)
                 .ToListAsync();

                totalCount = await _myheartContext.Client_Questionaire
                    .CountAsync();
            }
 
            return new DataTableResponse<Client_QuestionaireDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<Client_Questionaire>, IEnumerable<Client_QuestionaireDTO>>(questionaires).ToList()
            };
        }
        public async Task<ICollection<Client_QuestionAnswerDTO>> GetQuestions(int questionaireId)
        {
            var questions = await _myheartContext.Client_QuestionAnswers
                .Include(q => q.SymptomQuestion)
                    .ThenInclude(sq => sq.Diseases)
                        .ThenInclude(d => d.Disease)
                .Include(q => q.SymptomQuestion)
                    .ThenInclude(sq => sq.Symptoms)
                        .ThenInclude(s => s.Symptoms)
                .Where(q => q.Client_QuestionaireId == questionaireId)
                .ToListAsync();

            var questiosDtos = _mapper.Map<ICollection<Client_QuestionAnswerDTO>>(questions);

            foreach (var question in questiosDtos)
            {
                if (question.SymptomQuestion.Symptoms != null)
                {
                    var symptomIds = question.SymptomQuestion.Symptoms.Select(x => x.SymptomId).ToArray();
                    var symptoms = await _myheartContext.Symptoms
                        .Where(x => symptomIds.Contains(x.Id))
                        .Select(x => x.Name)
                        .ToListAsync();

                    question.SymptomQuestion.SymptomName = string.Join(", ", symptoms);
                }
            }

            return questiosDtos;
        }

        public async Task<Client_QuestionaireResultDTO> GetResult(int id)
        {
            var questionaire = await _myheartContext.Client_Questionaire
                .Include(q => q.QuestionAnswers)
                    .ThenInclude(qa => qa.SymptomQuestion)
                        .ThenInclude(sq => sq.Diseases)  
                            .ThenInclude(d => d.Disease)
                .Select(q => new  
                {
                    Id = q.Id,
                    QuestionAnswers = q.QuestionAnswers.Where(qa => qa.Approved)
                })
                .FirstOrDefaultAsync(q => q.Id == id);

            if (questionaire == null)
                return null;

            var result = new Client_QuestionaireResultDTO();

            var diseases = questionaire.QuestionAnswers.SelectMany(qa => qa.SymptomQuestion.Diseases).ToList();

            var diseaseAverageStrengthList = diseases.GroupBy(d => d.DiseaseId).Select(d => new { Id = d.Key, Average = d.Average(a => a.BondStrength) }).ToList();

            var probableDiseasIdList = diseaseAverageStrengthList.Where(d => d.Average > probableThreshold).Select(d => d.Id).ToList();

            result.ProbableDiseases = new List<Client_QuestionaireDiseaseAverageDTO>();

            foreach (var disease in diseases)
            {
                if (probableDiseasIdList.Contains(disease.DiseaseId) && !result.ProbableDiseases.Any(d => d.Disease.Id == disease.DiseaseId))
                {
                    var diseaseAverage = new Client_QuestionaireDiseaseAverageDTO
                    {
                        Disease = _mapper.Map<DiseaseDTO>(disease.Disease),
                        AverageBondStrength = Math.Round(diseaseAverageStrengthList.Single(x => x.Id == disease.DiseaseId).Average, 2)
                    };

                    result.ProbableDiseases.Add(diseaseAverage);
                }
            }

            var posibleDiseasIdList = diseaseAverageStrengthList.Where(d => d.Average >= possibleThreshold && d.Average <= probableThreshold).Select(d => d.Id);
            result.PossibleDiseases = new List<Client_QuestionaireDiseaseAverageDTO>();

            foreach (var disease in diseases)
            {
                if (posibleDiseasIdList.Contains(disease.DiseaseId) && !result.PossibleDiseases.Any(d => d.Disease.Id == disease.DiseaseId))
                {
                    var diseaseAverage = new Client_QuestionaireDiseaseAverageDTO
                    {
                        Disease = _mapper.Map<DiseaseDTO>(disease.Disease),
                        AverageBondStrength = Math.Round(diseaseAverageStrengthList.Single(x => x.Id == disease.DiseaseId).Average, 2)
                    };

                    result.PossibleDiseases.Add(diseaseAverage);
                }
            }

            var improbableDiseasIdList = diseaseAverageStrengthList.Where(d => d.Average < possibleThreshold).Select(d => d.Id);
            result.ImprobableDiseases = new List<Client_QuestionaireDiseaseAverageDTO>();

            foreach (var disease in diseases)
            {
                if (improbableDiseasIdList.Contains(disease.DiseaseId) && !result.ImprobableDiseases.Any(d => d.Disease.Id == disease.DiseaseId))
                {
                    var diseaseAverage = new Client_QuestionaireDiseaseAverageDTO
                    {
                        Disease = _mapper.Map<DiseaseDTO>(disease.Disease),
                        AverageBondStrength = Math.Round(diseaseAverageStrengthList.Single(x => x.Id == disease.DiseaseId).Average, 2)
                    };

                    result.ImprobableDiseases.Add(diseaseAverage);
                }
            }

            return result;
        }
    }
}
