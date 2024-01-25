using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class NonpharmaticTherapy_Disease_Indication : BaseModel
    {
        [Required]
        public int NonpharmaticTherapyId { get; set; }
        [Required]
        public int DiseaseId { get; set; }
        [Required]
        public int BondStrength { get; set; }
        public virtual NonpharmaticTherapy NonpharmaticTherapy { get; set; }
        public virtual Disease Disease { get; set; }
    }
}
