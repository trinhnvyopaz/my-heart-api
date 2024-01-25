using MyHeart.Data.Models;
using MyHeart.DTO;
using MyHeart.DTO.Questions;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> ListAsync(UType userType);
        Task<DataTableResponse<UserDTO>> GetUsersTable(GroupedDataTableRequest request);

        Task<UserDTO> CurrentUserAsync();
        Task<UserDTO> UserByIdAsync(int id);

        Task<UserDTO> RegisterUserAsync(RegisterDTO registerViewModel);

        Task<Dictionary<string, string>> Validate(RegisterDTO registerViewModel);

        Task<bool> SendRecoverPasswordLink(string email);

        Task<UserDTO> RecoverPassword(PasswordRecoverDTO recoverDTO);
        Task<string> MakePasswordRecoveryLink(User user, int recoveryTimeoutMinutes);

        Task<UserDTO> ActivateUser(Guid token);

        Task<IEnumerable<QuestionListDTO>> GetUserQuestions(int userId);

        Task<UserDetailDTO> GetUserDetail(int userId);

        Task<IEnumerable<QuestionCommentDTO>> GetQuestionComments(int questionId);

        Task<UserDTO> DeleteUser(int userId);

        Task<UserDetailDTO> UpdateUser(UserDetailDTO user, string userName);
        Task<IEnumerable<UserReportDTO>> GetUserReports(int userId);
        Task<UserReportDTO> AddUserReport(int userId, UserReportDTO report, string userName);
        Task<IEnumerable<UserReportFileDTO>> GetUserReportFiles(int userId, int reportId);
        Task<bool> DeleteUserReport(int userId, int reportId);
        Task<bool> UpdateReport(int userId, int reportId, UserReportUpdateDTO reportUpdateDTO);
        Task<UserNotificationSettingsDTO> GetUserNotificationSettings();
        Task<UserNotificationSettingsDTO> GetUserNotificationSettings(int userId);
        Task<UserNotificationSettingsDTO> UpdateUserNotificationSettings(UserNotificationSettingsDTO settings);
        Task<int> GetUserIdByEmail(string email);

        Task<bool> InvalidatePassword(int userId);

        Task<bool> EmailConfirmedById(int id);
        Task<bool> MFAConfirmedById(int id);
        Task<bool> AddNumberToUser(int id, string number);
        Task<UserFmcTokenDTO> AddFmcToken(UserFmcTokenDTO tokenDto);

        Task<IEnumerable<UserFullDTO>> GetForPatientsExport();

        Task<IEnumerable<UserFullDTO>> GetDoctorsForExport();
    }
}