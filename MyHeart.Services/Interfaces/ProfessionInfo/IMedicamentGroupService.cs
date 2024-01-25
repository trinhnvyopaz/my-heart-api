using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO.ProfessionInformation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IMedicamentGroupService
    {
        Task<IEnumerable<MedicamentGroupDTO>> MedicamentGroupListAsync();
        Task<MedicamentGroupDTO> MedicamentGroupAsync(int medicamentGroupId);
        Task<MedicamentGroupDTO> UpdateMedicamentGroup(MedicamentGroupDTO medicamentGroup, string user);
        Task<MedicamentGroupDTO> AddMedicamentGroupAsync(MedicamentGroupDTO medicamentGroup, string user);
        Task<MedicamentGroupDTO> DeleteMedicamentGroup(int medicamentGroupId);
        Task<Dictionary<string, string>> ValidateMedicamentGroup(MedicamentGroupDTO symptom);

        Task<IEnumerable<PharmacyDTO>> GetExcludedPharmacies(int medicamentGroupId);

        Task<DiscardedPharmaciesResponse<MedicamentGroup_Pharmacy_ByAtc>> GetDiscardedPharmacies(DiscardedPharmaciesRequest request);
        Task<MedicamentGroup_Pharmacy_ByAtc> TogglePharmacyDiscard(MedicamentGroup_Pharmacy_ByAtc pharmacy, string user);

        Task<DataTableResponse<MedicamentGroupDTO>> GetMedicamentGroupsTable(DataTableRequest request);
        Task<DataTableResponse<MedicamentGroupDTO>> GetPagedResource(SortedPagedSourceRequest request);
        Task<IEnumerable<MedicamentGroupDTO>> GetByIds(IEnumerable<int> ids);
    }
}