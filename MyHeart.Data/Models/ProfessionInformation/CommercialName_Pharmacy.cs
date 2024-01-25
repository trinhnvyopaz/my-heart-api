using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class CommercialName_Pharmacy : BaseModel
    {
        [Required]
        public int CommercialNamesId { get; set; }
        [Required]
        public int PharmacyId { get; set; }
        [Required]
        public int BondStrength { get; set; }
        public virtual CommercialName CommercialNames { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }
    }
}
