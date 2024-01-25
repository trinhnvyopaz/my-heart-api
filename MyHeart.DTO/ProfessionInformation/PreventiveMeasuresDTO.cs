using MyHeart.DTO.ProfessionInformation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class PreventiveMeasuresDTO : BaseEntityDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Disease_PreventiveMeasures_PreventiveMeasuresDTO> Indication { get; set; }
        public List<MedicalFacilities_PreventiveMeasures_PreventiveDTO> WorkspaceList { get; set; }
        public List<PreventiveMeasures_SynonymDTO> Synonyms { get; set; }
    }
}
