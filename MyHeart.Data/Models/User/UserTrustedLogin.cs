using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.Models {
    public class UserTrustedLogin : BaseModel {
        public int UserId { get; set; }
        public Guid SharedSecret { get; set; }

        public User User { get; set; }
    }
}
