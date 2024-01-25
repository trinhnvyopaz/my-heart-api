using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class NonpharmaticTherapy : BaseModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "decimal(18, 6)")]
        public decimal? Efficiency { get; set; }
        public string HospitalizationLength { get; set; }
        public ICollection<NonpharmaticTherapy_Disease_Indication> Indication { get; set; }
        public ICollection<NonpharmaticTherapy_MedicalFacilities_MedicalIntervention> MedicalIntervention { get; set; }
        public ICollection<NonpharmaticTherapy_Disease_Contraindication> Complication { get; set; }
        public ICollection<NonpharmaticTherapy_Synonyms> Synonyms { get; set; }

        public List<UserNonpharmacy> UserNonpharmacy { get; set; }
    }
}
