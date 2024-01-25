using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class Client_QuestionAnswer : BaseModel
    {
        [Required]
        public int SymptomQuestionId { get; set; }
        [Required]
        public bool Approved { get; set; }
        public int Client_QuestionaireId { get; set; }
        public virtual Client_Questionaire Client_Questionaire { get;set;}
        public virtual SymptomQuestions SymptomQuestion { get; set; }
    }
}
