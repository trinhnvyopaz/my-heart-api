using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class NonpharmaticTherapy_MedicalFacilities_MedicalInterventionDTO : BaseEntityDTO
    {
        public int NonpharmaticTherapyId { get; set; }
        public int MedicalFacilitiesId { get; set; }
        public int BondStrength { get; set; }
    }
}
