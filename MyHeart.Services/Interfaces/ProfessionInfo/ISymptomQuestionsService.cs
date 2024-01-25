using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO.ProfessionInformation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface ISymptomQuestionsService
    {
        Task<IEnumerable<SymptomQuestionsDTO>> SymptomQuestionsListAsync();
        Task<SymptomQuestionsDTO> SymptomQuestionsAsync(int SymptomQuestionsId);
        Task<SymptomQuestionsDTO> UpdateSymptomQuestions(SymptomQuestionsDTO SymptomQuestions, string user);
        Task<SymptomQuestionsDTO> AddSymptomQuestionsAsync(SymptomQuestionsDTO SymptomQuestions, string user);
        Task<SymptomQuestionsDTO> DeleteSymptomQuestions(int SymptomQuestionsId);
        Task<Dictionary<string, string>> ValidateSymptomQuestions(SymptomQuestionsDTO SymptomQuestions);
        Task<IEnumerable<SymptomQuestionsDTO>> SymptomQuestionsListByDiseaseIdAsync(int diseaseId);
        Task<DataTableResponse<SymptomQuestionsDTO>> GetSymptomQuestionsTable(DataTableRequest request);
        Task<IEnumerable<SymptomQuestionsDTO>> GetSymptomQuestionsBySymptomIds(IEnumerable<int> symptomIds);
        Task<IEnumerable<SymptomQuestions_DiseaseDTO>> UpdateSymptomQuestionsDiseases(IEnumerable<SymptomQuestions_DiseaseDTO> questionDiseases, string user);
        Task<DataTableResponse<SymptomQuestionsDTO>> GetPagedResource(SortedPagedSourceRequest request);
        Task<IEnumerable<SymptomQuestionsDTO>> GetSymptomQuestionsByIds(IEnumerable<int> ids);
    }
}