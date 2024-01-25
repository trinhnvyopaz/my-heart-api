using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.Models
{
    public class QuestionComment : BaseModel
    {
        public string Text { get; set;  }
        public int UserId { get; set; }
        public User User { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
