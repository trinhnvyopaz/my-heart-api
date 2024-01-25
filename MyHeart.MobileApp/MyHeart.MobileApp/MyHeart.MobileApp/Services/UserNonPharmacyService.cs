using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using MyHeart.MobileApp.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.MobileApp.Services
{
    public class UserNonPharmacyService : BaseService
    {
        private const string Endpoint = "UserNonpharmacy/";

        public async Task<IEnumerable<UserNonpharmacyDto>> GetUserNonpharmacies()
        {
            var response = await restService.SendRequest<IEnumerable<UserNonpharmacyDto>>($"{Endpoint}", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }          
        }

        public async Task<UserNonpharmacyDto> SaveUserNonpharmacy(UserNonpharmacyViewModel vm)
        {
            var dto = MapVmToDto(vm);

            var response = await restService.SendRequest<UserNonpharmacyDto>($"{Endpoint}", HttpMethod.Post, dto);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<NonpharmacyDTO>> GetNonPharmaticTherapyByName(string query)
        {
            var response = await restService.SendRequest<IEnumerable<NonpharmacyDTO>>($"{Endpoint}getNonpharmaticTherapiesByName/{query}", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteNonPharmaticTherapies(int id)
        {
            var response = await restService.SendRequest<IEnumerable<NonpharmacyDTO>>($"{Endpoint}{id}", HttpMethod.Delete, null);

            return response.IsSuccess;
        }

        private UserNonpharmacyDto MapVmToDto(UserNonpharmacyViewModel vm)
        {
            return new UserNonpharmacyDto
            {
                Id = vm.Id,
                FacilityName = vm.FacilityName,
                NonpharmaticTherapy = vm.NonpharmaticTherapy,
                NonpharmaticTherapyId = vm.NonpharmaticTherapyId,
                Note = vm.Note,
                User = vm.User,
                UserId = vm.UserId
            };
        }
    }
}
