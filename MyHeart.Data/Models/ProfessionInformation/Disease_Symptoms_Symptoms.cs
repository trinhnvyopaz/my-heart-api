using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class Disease_Symptoms_Symptoms : BaseModel
    {

        [Required]
        public int DiseaseId { get; set; }
        [Required]
        public int SymptomsId { get; set; }
        [Required]
        public int BondStrength { get; set; }
        public virtual Symptoms Symptoms { get; set; }
        public virtual Disease Disease { get; set; }
    }
}
