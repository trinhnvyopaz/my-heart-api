using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.ViewModels
{
    public class UserDiseaseViewModel : BaseViewModel
    {
        private DateTime startDate;
        private string note;
        private string starDateString;

        public int Id { get; set; }
        public int UserId { get; set; }
        public UserDTO User { get; set; }

        public int DiseaseId { get; set; }
        public DiseaseDTO Disease { get; set; }

        public DateTime StartDate 
        { 
            get => startDate; 
            set => SetProperty(ref startDate, value);
        }
        public string StartDateString 
        { 
            get => starDateString; 
            set => SetProperty(ref starDateString, value); 
        }
        public string Note 
        { 
            get => note;
            set => SetProperty(ref note, value); 
        }
        public UserDiseaseViewModel()
        {

        }
        public UserDiseaseViewModel(UserDiseaseDto dto)
        {
            Id = dto.Id;
            UserId = dto.UserId;
            User = dto.User;
            DiseaseId = dto.DiseaseId;
            Disease = dto.Disease;
            StartDate = dto.StartDate;
            StartDateString = dto.StartDateString;
            Note = dto.Note;
        }
        public UserDiseaseViewModel(DiseaseDTO dto)
        {
            DiseaseId = dto.Id;
            Disease = dto;
            StartDate = DateTime.Now;
        }
    }
}
