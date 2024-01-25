using MyHeart.Data.Models.ProfessionInformation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class Disease : BaseModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public double? TargetWeight { get; set; }
        public int? TargetSystolicPressure { get; set; }
        public int? TargetDiastolicPressure { get; set; }
        public int? TargetHeartRate { get; set; }
        public double? TargetLdl { get; set; }
        public string SystemicMeasures { get; set; }
        public int? Frequency { get; set; }
        public ICollection<Disease_Symptoms_Symptoms> Symptoms { get; set; }
        public ICollection<Disease_Disease_Causes> Causes { get; set; }
        public ICollection<MedicamentGroup_Disease_Indication> MedicamentGroup { get; set; }
        public ICollection<Disease_NonpharmaticTherapy_NonpharmaticTherapy> NonpharmaticTherapy { get; set; }
        public ICollection<Disease_PreventiveMeasures_PreventiveMeasures> PreventiveMeasures { get; set; }
        public virtual ICollection<SymptomQuestions_Disease> SymptomQuestions { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<Disease_Synonyms> Synonyms { get; set; }
    }
}
