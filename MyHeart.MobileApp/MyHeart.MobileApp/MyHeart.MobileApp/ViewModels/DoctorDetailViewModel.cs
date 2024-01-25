using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.ViewModels
{
    public class DoctorDetailViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public string Workplace { get; set; }

        public DoctorDetailViewModel(DoctorDetailDTO dto)
        {
            Id = dto.Id;
            UserId = dto.UserId;
            Firstname = dto.FirstName;
            Lastname = dto.LastName;
            Email = dto.Email;
            Phone = dto.Phone;
            Description = dto.Description;
            Workplace = dto.WorkPlace;
        }
    }
}
