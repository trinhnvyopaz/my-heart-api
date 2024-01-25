using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class TherapyNewsDTO : BaseEntityDTO
    {
        [Required]
        public string Text { get; set; }
        public string Description { get; set; }
        [Required]
        public string WebLink { get; set; }
        public List<TherapyNews_Disease_SickDTO> Diseases { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class TherapyNewsSubscriptionSettingsDto
    {
        public int UserId { get; set; }
        public int SubscriptionPreferences { get; set; }
        public bool TherapyNewsEmailNotification { get; set; }
    }
}
