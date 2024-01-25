using MyHeart.Data;
using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface INonpharmacyService
    {

        Task<IEnumerable<NonpharmacyDTO>> NonpharmacyListAsync();
        Task<NonpharmacyDTO> NonpharmacyAsync(int nonpharmacyId);
        Task<NonpharmacyDTO> UpdateNonpharmacy(NonpharmacyDTO nonpharmacy, string user);
        Task<NonpharmacyDTO> AddNonpharmacyAsync(NonpharmacyDTO nonpharmacy, string user);
        Task<NonpharmacyDTO> DeleteNonpharmacy(int nonpharmacyId);
        Task<Dictionary<string, string>> ValidateNonpharmacy(NonpharmacyDTO symptom);
        Task<DataTableResponse<NonpharmacyDTO>> GetNonpharmacyTable(DataTableRequest request);
        Task<DataTableResponse<NonpharmacyDTO>> GetPagedResource(SortedPagedSourceRequest request);
        Task<IEnumerable<NonpharmacyDTO>> GetByIds(IEnumerable<int> ids);
        Task<IEnumerable<UserNonpharmacyDto>> GetAllUserNonpharmacy();
    }
}