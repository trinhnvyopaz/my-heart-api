using MyHeart.Data.Models;
using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services
{
    public interface IUserNonpharmacyService
    {
        Task<IEnumerable<UserNonpharmacyDto>> GetUserNonpharmacies();
        Task<IEnumerable<UserNonpharmacyDto>> GetUserNonpharmacies(int id);
        Task<bool> DoesNonPharmacyBelongToUser(int userId, int nonPharmaId);
        Task<UserNonpharmacyDto> SaveUserNonpharmacy(UserNonpharmacyDto model, string userName);
        Task<bool> DeleteUserNonpharmacy(int id);
        Task<IEnumerable<NonpharmacyDTO>> GetNonpharmaticTherapiesByName(string searchString);
    }
}
