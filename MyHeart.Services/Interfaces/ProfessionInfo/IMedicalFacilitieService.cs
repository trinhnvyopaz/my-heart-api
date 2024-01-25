using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IMedicalFacilitieService
    {
       
        Task<IEnumerable<MedicalFacilitiesDTO>> MedicalFacilitiesListAsync();
        Task<MedicalFacilitiesDTO> MedicalFacilitiesAsync(int medicalFacilitiesId);
        Task<MedicalFacilitiesDTO> UpdateMedicalFacilities(MedicalFacilitiesDTO medicalFacilities, string user);
        Task<MedicalFacilitiesDTO> AddMedicalFacilitiesAsync(MedicalFacilitiesDTO medicalFacilities, string user);
        Task<bool> AddOrUpdateMedicalFacility(MedicalFacilitiesDTO facility, string user);
        Task<MedicalFacilitiesDTO> DeleteMedicalFacilities(int medicalFacilitiesId);
        Task<Dictionary<string, string>> ValidateMedicalFacilities(MedicalFacilitiesDTO medicalFacilities);
        Task<DataTableResponse<MedicalFacilitiesDTO>> GetMedicalFacilitiesTable(DataTableRequest request);
        Task<IEnumerable<MedicalFacilitiesDTO>> GetByIds(IEnumerable<int> ids);
        Task<DataTableResponse<MedicalFacilitiesDTO>> GetPagedResource(SortedPagedSourceRequest request);

        Task<bool> BulkInsertOrUpdate(List<MedicalFacilitiesDTO> facilities, string user);
        Task<bool> BulkDelete(List<MedicalFacilitiesDTO> facilities);
    }
}