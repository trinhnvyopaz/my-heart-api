using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class Pharmacy_CommercialNames : BaseModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int PharmacyId { get; set; }
        public virtual Pharmacy MedicamentGroup { get; set; }
    }
}
