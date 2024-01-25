using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class MedicamentGroup_Disease_Indication : BaseModel
    {
        public int MedicamentGroupId { get; set; }
        public int DiseaseId { get; set; }
        public int BondStrength { get; set; }
        public virtual MedicamentGroup MedicamentGroup { get; set; }
        public virtual Disease Indication { get; set; }

    }
}
