using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class Pharmacy_Disease_IndicationDTO : BaseEntityDTO
    {
        [Required]
        public int PharmacyId { get; set; }
        [Required]
        public int DiseaseId { get; set; }
        public int BondStrength { get; set; }
    }
}
