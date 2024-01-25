using MyHeart.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyHeart.Services
{
    public interface IQuestionaireService
    {
        Task<Client_QuestionaireDTO> CreateQuestionaire(Client_QuestionaireDTO questionaire);
        Task<DataTableResponse<Client_QuestionaireDTO>> GetQuestionaireTable(DataTableRequest request);
        Task<ICollection<Client_QuestionAnswerDTO>> GetQuestions(int questionaireId);
        Task<Client_QuestionaireResultDTO> GetResult(int id);
    }
}