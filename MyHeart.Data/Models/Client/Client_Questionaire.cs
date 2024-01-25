using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.Models
{
    public class Client_Questionaire : BaseModel
    {
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual User User { get; set; }
        public ICollection<Client_QuestionAnswer> QuestionAnswers { get; set; }
    }
}
