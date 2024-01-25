using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.ProfessionInformation
{
    public class Symptom_SynonymsDTO : SynonymBaseDTO
    {
        public int SymptomId { get; set; }
        public SymptomDTO Symptom { get; set; }
    }
}
