using MyHeart.DTO;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces.Client
{
    public interface IUserReportsService
    {
        Task<IEnumerable<UserReportDTO>> GetReports();
        Task<IEnumerable<UserReportDTO>> GetReports(int id);
        Task<UserReportDTO> SaveReport(UserReportDTO model, string userName);
        Task<bool> DeleteUserReport(int id);
        Task<bool> DoesReportBelongToUser(int userId, int reportId);
        Task<bool> DoesReportFileBelongToUser(int userId, int fileId);
        Task<UserReportFileDTO> GetUserReportFile(int reportFileId);
        Task<UserReportDTO> UpdateReport(UserReportDTO model, string userName);

        Task<DataTableResponse<UserReportDTO>> GetAllReports(DataTableRequest request);
    }
}
