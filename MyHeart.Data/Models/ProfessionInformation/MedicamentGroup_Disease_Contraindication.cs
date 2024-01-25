using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class MedicamentGroup_Disease_Contraindication : BaseModel
    {
        [Required]
        public int MedicamentGroupId { get; set; }
        [Required]
        public int DiseaseId { get; set; }
        public virtual MedicamentGroup MedicamentGroup { get; set; }
        public virtual Disease Contraindication { get; set; }

    }
}
