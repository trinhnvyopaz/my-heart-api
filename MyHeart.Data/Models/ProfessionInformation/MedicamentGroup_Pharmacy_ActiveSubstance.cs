using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class MedicamentGroup_Pharmacy_ActiveSubstance : BaseModel
    {
        [Required]
        public int MedicamentGroupId { get; set; }
        [Required]
        public int PharmacyId { get; set; }
        [Required]
        public int BondStrength { get; set; }
        public virtual MedicamentGroup MedicamentGroup { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }

    }
}
