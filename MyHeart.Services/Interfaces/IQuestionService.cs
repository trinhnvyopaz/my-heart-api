using MyHeart.DTO;
using MyHeart.DTO.Questions;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<List<QuestionListDTO>> GetListAsync();
        Task<List<QuestionListDTO>> GetOpenUserQuestions(int userId);
        Task<List<QuestionListDTO>> GetClosedUserQuestions(int userId);
        Task<DataTableResponse<QuestionListDTO>> GetOpenUserQuestionsDataTable(int userId, DataTableRequest request);
        Task<DataTableResponse<QuestionListDTO>> GetClosedUserQuestionsDataTable(int userId, DataTableRequest request);
        Task<QuestionFullDto> GetQuestionById(int questionId, bool includeQuestions = false);
        Task<QuestionListDTO> AskQuestion(int userId, QuestionListDTO question, string userName);
        Task<QuestionCommentDTO> ReplyToQuestion(QuestionCommentDTO reply);
        Task<bool> ArchiveQuestion(int QuestionId, UserDTO user);
        Task<bool> DoesQuestionBelongToUser(int questionId, int userId);
        Task<DataTableResponse<QuestionListDTO>> GetQuestionsTable(GroupedDataTableRequest request);
        Task<DataTableResponse<QuestionCommentDTO>> GetLastCommentsOfQuestion(int questionId, int page, int pageSize);
        Task<VideoRoomDTO> GetVideoRoomDetails(int questionId);
        Task<IEnumerable<QuestionFullDto>> GetListFullAsync();
        Task<IEnumerable<QuestionCommentDTO>> GetClosedQuestionCommentsByDoctorIds(int[] doctorIds);
        Task<bool> DeleteQuestion(int id);
    }
}
