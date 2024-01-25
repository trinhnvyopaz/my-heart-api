using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class Pharmacy_Symptoms_SideEffects : BaseModel
    {
        [Required]
        public int PharmacyId { get; set; }
        [Required]
        public int SymptomId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }
        public virtual Symptoms Symptoms { get; set; }
    }
}
