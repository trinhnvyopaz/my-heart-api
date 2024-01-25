using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.ViewModels
{
    public class TherapyNewsViewModel : BaseViewModel
    {
        private bool isDetailExpanded;

        public int Id { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string LastUpdateUser { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string WebLink { get; set; }
        public List<TherapyNews_Disease_SickDTO> Diseases { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsDetailExpanded
        {
            get => isDetailExpanded;
            set => SetProperty(ref isDetailExpanded, value);
        }

        public TherapyNewsViewModel(TherapyNewsDTO dto)
        {
            Id = dto.Id;
            Text = dto.Text;
            Description = dto.Description;
            WebLink = dto.WebLink;
            Diseases = dto.Diseases;
            CreateDate = dto.CreateDate;
            IsDetailExpanded = false;
            LastUpdateDate = dto.LastUpdateDate;
        }
    }
}
