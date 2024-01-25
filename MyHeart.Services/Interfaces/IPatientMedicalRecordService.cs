using MyHeart.DTO;
using MyHeart.DTO.MedicalReport;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IPatientMedicalRecordService
    {
        public Task<DataTableResponse<PatientMedicalRecordDTO>> Search(DataTableRequest request);
        public Task<PatientMedicalRecordDTO> Get(int id);

        public Task<PatientMedicalRecordDTO> Create(PatientMedicalRecordDTO medicalReportDTO, string orcText);
        public Task<PatientMedicalRecordDTO> ScanReportFile(PatientMedicalRecordDTO medicalReportDTO, string orcText, string orcTextURL);

        public Task<PatientMedicalRecordDTO> Update(
            int id,
            PatientMedicalRecordDTO medicalReportDTO
        );

        public Task<PatientMedicalRecordDTO> Delete(int id);
        Task<FileDTO> ExportExcel(int id);

    }
}
