using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO
{
    public class NotificationDto : BaseEntityDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? DiseaseId { get; set; }
        public DiseaseDTO Disease { get; set; }
    }
}
