using MyHeart.Data.ModelExtensions;
using MyHeart.DTO.Questions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.Models
{
    public partial class Question : BaseModel, ISoftDeletable
    {
        public string Subject { get; set; }
        public QuestionStatus Status { get; set; }
        public DateTime CreationDate { get; set; }
        public int UserId { get; set; }
        public List<QuestionComment> Comments { get; set; }
        public virtual User User { get; set; }
        public bool IsDeleted { get; set; }
    }
}
