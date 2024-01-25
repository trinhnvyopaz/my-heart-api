using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class Disease_PreventiveMeasures_PreventiveMeasures : BaseModel
    {

        [Required]
        public int DiseaseId { get; set; }
        [Required]
        public int PreventiveMeasuresId { get; set; }
        public int BondStrength { get; set; }
        public virtual Disease Disease { get; set; }
        public virtual PreventiveMeasures PreventiveMeasures { get; set; }
    }
}
