using MyHeart.DTO.ProfessionInformation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class SymptomQuestionsDTO : BaseEntityDTO
    {
        public string Text { get; set; }
        public List<SymptomQuestions_SymptomDTO> Symptoms { get; set; }
        public List<SymptomQuestions_DiseaseDTO> Diseases { get; set; }
        public string SymptomName { get; set; }
        public string DiseaseString { get; set; }
    }
}
