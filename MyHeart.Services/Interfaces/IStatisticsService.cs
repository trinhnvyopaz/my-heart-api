using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IStatisticsService
    {
        Task<FileDTO> ExportPacients();
        Task<FileDTO> ExportQuestions();
        Task<FileDTO> ExportDoctors();
        Task<FileDTO> ExportDiseases();
        Task<FileDTO> ExportPharmacy();
        Task<FileDTO> ExportNonpharmacy();
        byte[] ExportToExcelNew<T>(IList<T> list, FileInfo file);
    }
}