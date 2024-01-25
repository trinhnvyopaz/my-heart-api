using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class NonpharmacyDTO : BaseEntityDTO
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Efficiency { get; set; }
        public string HospitalizationLength { get; set; }
        public List<NonpharmaticTherapy_Disease_IndicationDTO> Indication { get; set; }
        public List<NonpharmaticTherapy_MedicalFacilities_MedicalInterventionDTO> MedicalIntervention { get; set; }
        public List<NonpharmaticTherapy_Disease_ContraindicationDTO> Complication { get; set; }
        public List<NonpharmaticTherapy_SynonymsDTO> Synonyms { get; set; }
    }
}

