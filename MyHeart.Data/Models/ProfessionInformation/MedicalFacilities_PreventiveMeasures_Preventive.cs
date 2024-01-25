using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class MedicalFacilities_PreventiveMeasures_Preventive : BaseModel
    {
        [Required]
        public int MedicalFacilitiesId { get; set; }
        [Required]
        public int PreventiveMeasureId { get; set; }
        [Required]
        public int BondStrength { get; set; }

        public virtual MedicalFacilities MedicalFacilities { get; set; }
        public virtual PreventiveMeasures PreventiveMeasure { get; set; }
    }
}
