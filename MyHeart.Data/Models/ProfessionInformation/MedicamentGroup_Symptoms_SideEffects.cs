using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class MedicamentGroup_Symptoms_SideEffects : BaseModel
    {
        [Required]
        public int MedicamentGroupId { get; set; }
        [Required]
        public int SymptomsId { get; set; }
        public int BondStrength { get; set; }
        public virtual MedicamentGroup MedicamentGroup { get; set; }
        public virtual Symptoms Symptoms { get; set; }
    }
}
