using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class Pharmacy_Symptoms_SideEffectsDTO : BaseEntityDTO
    {
        [Required]
        public int PharmacyId { get; set; }
        [Required]
        public int SymptomId { get; set; }
    }
}
