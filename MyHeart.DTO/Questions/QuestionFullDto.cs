using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.Questions
{
    public class QuestionFullDto : QuestionListDTO
    {
        public IEnumerable<QuestionCommentFullDTO> Comments { get; set; }
        public UserFullDTO User { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
