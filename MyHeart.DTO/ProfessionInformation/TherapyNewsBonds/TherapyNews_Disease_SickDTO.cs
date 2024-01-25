using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class TherapyNews_Disease_SickDTO : BaseEntityDTO
    {
        [Required]
        public int TherapyNewsId { get; set; }
        [Required]
        public int DiseaseId { get; set; }
    }
}
