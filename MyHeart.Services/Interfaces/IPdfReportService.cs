using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IPdfReportService
    {
        Task<FileDTO> GetHealthsStatusReportAsync(int userId);
        Task<FileDTO> GetPharmaciesReportAsync(int userId);

        Task<FileDTO> GetChatReportDto(int userId, int questionid);
    }
}