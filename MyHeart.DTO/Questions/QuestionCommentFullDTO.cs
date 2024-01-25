using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.Questions
{
    public class QuestionCommentFullDTO : QuestionCommentDTO
    {
        public UserDTO Sender { get; set; }
    }
}
