using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IPharmacyService
    {
        Task<IEnumerable<PharmacyDTO>> PharmacyListAsync();
        Task<PharmacyDTO> PharmacyAsync(int pharmacyId);
        Task<IEnumerable<PharmacyDTO>> PharmacyAsync(string nameRegPart);
        Task<PharmacyDTO> UpdatePharmacy(PharmacyDTO pharmacy, string user);
        Task<PharmacyDTO> AddPharmacyAsync(PharmacyDTO pharmacy, string user);
        Task<bool> AddOrUpdatePharmacy(PharmacyDTO pharmacy, string user);
        Task<PharmacyDTO> DeletePharmacy(int pharmacyId);
        Task<Dictionary<string, string>> ValidatePharmacy(PharmacyDTO symptom);

        Task<DataTableResponse<PharmacyDTO>> GetPharmaciesTable(DataTableRequest request);
        Task<DataTableResponse<PharmacyDTO>> GetPagedResource(SortedPagedSourceRequest request);
        Task<IEnumerable<PharmacyDTO>> GetByIds(IEnumerable<int> ids);

        Task<bool> BulkInsertOrUpdate(List<PharmacyDTO> pharmacies, string user);
        Task<bool> BulkDelete(List<PharmacyDTO> pharmacies);
        Task<IEnumerable<UserPharmacyDTO>> GetAllUserPharmacy();
    }
}