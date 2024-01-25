using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class TherapyNews_Disease_Sick : BaseModel
    {
        [Required]
        public int TherapyNewsId { get; set; }
        [Required]
        public int DiseaseId { get; set; }
        public virtual Disease Disease { get; set; }
        public virtual TherapyNews TherapyNews { get; set; }
    }
}
