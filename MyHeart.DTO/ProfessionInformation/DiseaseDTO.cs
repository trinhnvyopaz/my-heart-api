using MyHeart.DTO.ProfessionInformation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class DiseaseDTO : BaseEntityDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public double? TargetWeight { get; set; }
        public int? TargetSystolicPressure { get; set; }
        public int? TargetDiastolicPressure { get; set; }
        public int? TargetHeartRate { get; set; }
        public double? TargetLdl { get; set; }
        public string SystemicMeasures { get; set; }

        public string Frequency { get; set; }
        public List<Disease_Symptoms_SymptomsDTO> Symptoms { get; set; }
        public List<Disease_Disease_CausesDTO> Causes { get; set; }
        public List<MedicamentGroup_Disease_IndicationDTO> MedicamentGroup { get; set; }
        public List<Disease_NonpharmaticTherapy_NonpharmaticTherapyDTO> NonpharmaticTherapy { get; set; }
        public List<Disease_PreventiveMeasures_PreventiveMeasuresDTO> PreventiveMeasures { get; set; }
        public List<Disease_SynonymsDTO> Synonyms { get; set; }
    }
}
