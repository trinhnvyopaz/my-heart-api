using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO
{
    public class Client_QuestionAnswerDTO : BaseEntityDTO
    { 
        public int SymptomQuestionId { get; set; }
        public SymptomQuestionsDTO SymptomQuestion { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Approved { get; set; }
    }
}
