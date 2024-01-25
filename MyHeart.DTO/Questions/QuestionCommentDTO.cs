using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.Questions
{
    public class QuestionCommentDTO : BaseEntityDTO
    {
        public string Text { get; set; }
        public int SenderId { get; set; }
        public int QuestionId { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
