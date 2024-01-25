using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class SymptomQuestions_SymptomDTO : BaseEntityDTO
    {
        public int SymptomId { get; set; }
        public int SymptomQuestionsId { get; set; }
        public int BondStrength { get; set; }
        public SymptomDTO Symptom { get; set; }
    }
}
