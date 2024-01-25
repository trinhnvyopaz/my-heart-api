using MyHeart.DTO.Client;
using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO.ProfessionInformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO
{
    public class Client_QuestionaireResultDTO
    {
        public ICollection<Client_QuestionaireDiseaseAverageDTO> ProbableDiseases { get; set; }
        public ICollection<Client_QuestionaireDiseaseAverageDTO> PossibleDiseases { get; set; }
        public ICollection<Client_QuestionaireDiseaseAverageDTO> ImprobableDiseases { get; set; }
    }
}
