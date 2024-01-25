using MyHeart.DTO.ProfessionInformation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class SymptomDTO : BaseEntityDTO
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Disease_Symptoms_SymptomsDTO> Diseases { get; set; }

        public ICollection<SymptomQuestions_SymptomDTO> SymptomQuestions { get; set; }
        public ICollection<Symptom_SynonymsDTO> Synonyms { get; set; }

    }
}
