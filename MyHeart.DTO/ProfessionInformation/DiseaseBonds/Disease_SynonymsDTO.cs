using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.ProfessionInformation
{
    public class Disease_SynonymsDTO : SynonymBaseDTO
    {
        public int DiseaseId { get; set; }
        public DiseaseDTO Disease { get; set; }
    }
}
