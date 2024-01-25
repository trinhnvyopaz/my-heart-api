using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.Questions
{
    public class QuestionListDTO : BaseEntityDTO
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public QuestionStatus Status { get; set; }
        public string CreationDate { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }

    }

    public enum QuestionStatus {
        Open,
        AwaitingPatientResponse,
        AwaitingDoctorResponse,
        Closed
    }
}
