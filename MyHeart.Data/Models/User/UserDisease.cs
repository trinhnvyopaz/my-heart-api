using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.Models
{
    public class UserDisease : BaseModel
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int DiseaseId { get; set; }
        public Disease Disease { get; set; }

        public DateTime StartDate { get; set; }
        public string StartDateString { get; set; }
        public string Note { get; set; }
    }
}
