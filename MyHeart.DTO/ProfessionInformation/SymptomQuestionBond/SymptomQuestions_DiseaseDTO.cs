using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.ProfessionInformation {
    public class SymptomQuestions_DiseaseDTO : BaseEntityDTO
    {
        public int DiseaseId { get; set; }
        public int SymptomQuestionsId { get; set; }
        public int BondStrength { get; set; }
        public DiseaseDTO Disease { get; set; }
    }
}
