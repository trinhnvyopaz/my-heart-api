using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.Client
{
    public class Client_QuestionaireDiseaseAverageDTO
    {
        public DiseaseDTO Disease { get; set; }
        public double AverageBondStrength { get; set; }
    }
}
