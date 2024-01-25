
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.Data.Models.ProfessionInformation
{
    public class MedicamentGroup_Atc : BaseModel
    {
        [Required]
        public int MedicamentGroupId { get; set; }
        [Required]
        public int AtcId { get; set; }
        public virtual MedicamentGroup MedicamentGroup { get; set; }
        public virtual Atc Atc { get; set; }
    }
}