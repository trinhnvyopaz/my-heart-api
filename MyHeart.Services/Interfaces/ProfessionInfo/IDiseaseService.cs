using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IDiseaseService
    {
        Task<IEnumerable<DiseaseDTO>> DiseaseListAsync();
        Task<DiseaseDTO> DiseaseAsync(int diseaseId);
        Task<DiseaseDTO> UpdateDisease(DiseaseDTO disease, string user);
        Task<DiseaseDTO> AddDiseaseAsync(DiseaseDTO disease, string user);
        Task<DiseaseDTO> DeleteDisease(int diseaseId);
        Task<Dictionary<string, string>> ValidateDisease(DiseaseDTO symptom);
        Task<DataTableResponse<DiseaseDTO>> GetDiseasesTable(DataTableRequest request);
        Task<IEnumerable<DiseaseDTO>> GetByIds(IEnumerable<int> ids);
        Task<DataTableResponse<DiseaseDTO>> GetPagedResource(SortedPagedSourceRequest request);
        Task<IEnumerable<UserDiseaseDto>> GetAllUserDiseases();
    }
}