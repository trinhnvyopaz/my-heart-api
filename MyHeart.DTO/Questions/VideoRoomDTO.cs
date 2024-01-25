using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.Questions {
    public class VideoRoomDTO : BaseEntityDTO {
        public int QuestionID { get; set; }
        public Guid RoomId { get; set; }
        public string Password { get; set; }
    }
}
