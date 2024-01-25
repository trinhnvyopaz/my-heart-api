using MyHeart.Data.Models.ProfessionInformation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class PreventiveMeasures : BaseModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Disease_PreventiveMeasures_PreventiveMeasures> Indication { get; set; }
        public ICollection<MedicalFacilities_PreventiveMeasures_Preventive> WorkspaceList { get; set; }
        public ICollection<PreventiveMeasures_Synonyms> Synonyms { get; set; }
    }
}
