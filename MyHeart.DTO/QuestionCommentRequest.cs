using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO {
    public class QuestionCommentRequest {
        public int QuestionId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
