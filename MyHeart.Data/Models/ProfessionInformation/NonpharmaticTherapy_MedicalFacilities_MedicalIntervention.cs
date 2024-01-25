using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class NonpharmaticTherapy_MedicalFacilities_MedicalIntervention : BaseModel
    {
        [Required]
        public int NonpharmaticTherapyId { get; set; }
        [Required]
        public int MedicalFacilitiesId { get; set; }
        [Required]
        public int BondStrength { get; set; }
        public virtual NonpharmaticTherapy NonpharmaticTherapy { get; set; }
        public virtual MedicalFacilities MedicalFacilities { get; set; }
    }
}
