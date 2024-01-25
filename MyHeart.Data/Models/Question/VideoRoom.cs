using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.Models {
    public class VideoRoom : BaseModel {
        public int QuestionID { get; set; }
        public Guid RoomId { get; set; }
        public string Password { get; set; }
    }
}
