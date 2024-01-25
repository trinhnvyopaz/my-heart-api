using MyHeart.DTO.User;
using MyHeart.MobileApp.ViewModels;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.MobileApp.Services
{
    public class DoctorService : BaseService
    {
        const string EndPoint = "Doctor/";

        public async Task<DoctorDetailDTO> GetDoctorDetailAsync(int userId)
        {
            var response = await restService.SendRequest<DoctorDetailDTO>($"{EndPoint}ByUserId/{userId}", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> UpdateAsync(DoctorDetailViewModel doctor)
        {
            var dto = MapVmToDto(doctor);

            var response = await restService.SendRequest($"{EndPoint}update", HttpMethod.Patch, dto);

            return response.IsSuccess;
        }

        private DoctorDetailDTO MapVmToDto(DoctorDetailViewModel doctor)
        {
            var dto = new DoctorDetailDTO
            {
                Id = doctor.Id,
                UserId = doctor.UserId,
                Description = doctor.Description,
                WorkPlace = doctor.Workplace,
                FirstName = doctor.Firstname,
                LastName = doctor.Lastname,
                Email = doctor.Email,
                Phone = doctor.Phone
            };

            return dto;
        }
    }
}
