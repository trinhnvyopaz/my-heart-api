using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.Data.Models
{
    public class MedicamentGroup_Pharmacy_Discard : BaseModel
    {
        [Required]
        public int MedicamentGroupId { get; set; }
        [Required]
        public int PharmacyId { get; set; }
        public virtual MedicamentGroup MedicamentGroup { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }
    }
}
