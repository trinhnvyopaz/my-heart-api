using MyHeart.Data.Models;
using MyHeart.DTO.User;
using MyHeart.DTO.Doctor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyHeart.DTO;

namespace MyHeart.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<UserDTO> RegisterUserAsync(RegisterDoctorDTO registerViewModel);

        Task<Dictionary<string, string>> Validate(RegisterDoctorDTO registerViewModel);

        Task<DoctorDetailDTO> UpdateDoctor(DoctorDetailDTO user, string userName);
        Task<DoctorDetailDTO> GetDoctorDetail(int doctorId);
        Task<DoctorDetailDTO> GetDoctorDetailByUserID(int userID);
    }
}