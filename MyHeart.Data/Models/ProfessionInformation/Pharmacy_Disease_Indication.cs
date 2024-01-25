using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class Pharmacy_Disease_Indication : BaseModel
    {
        [Required]
        public int PharmacyId { get; set; }
        [Required]
        public int DiseaseId { get; set; }
        public int BondStrength { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }
        public virtual Disease Indication { get; set; }
    }
}
