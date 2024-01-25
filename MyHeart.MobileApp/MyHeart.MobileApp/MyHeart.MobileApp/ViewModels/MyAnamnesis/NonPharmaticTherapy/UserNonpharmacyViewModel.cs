using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.ViewModels
{
    public class UserNonpharmacyViewModel : BaseViewModel, ICloneable
    {
        private string facilityName;
        private string note;

        public int Id { get; set; }
        public int UserId { get; set; }
        public UserDTO User { get; set; }
        public int NonpharmaticTherapyId { get; set; }
        public NonpharmacyDTO NonpharmaticTherapy { get; set; }

        public string FacilityName 
        { 
            get => facilityName;
            set => SetProperty(ref facilityName, value);
        }
        public string Note 
        {
            get => note;
            set => SetProperty(ref note, value);
        }

        public UserNonpharmacyViewModel(UserNonpharmacyDto dto)
        {
            Id = dto.Id;
            UserId = dto.UserId;
            User = dto.User;
            NonpharmaticTherapyId = dto.NonpharmaticTherapyId;
            NonpharmaticTherapy = dto.NonpharmaticTherapy;
            FacilityName = dto.FacilityName;
            Note = dto.Note;
        }
        public UserNonpharmacyViewModel(NonpharmacyDTO dto)
        {
            NonpharmaticTherapy = dto;
            NonpharmaticTherapyId = dto.Id;
        }

        public object Clone()
        {
            return base.MemberwiseClone();
        }
    }
}
