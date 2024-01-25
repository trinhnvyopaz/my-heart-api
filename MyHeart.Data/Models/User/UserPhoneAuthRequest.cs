using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.Models {
    public class UserPhoneAuthRequest : BaseModel {
        public Guid LoginSecret { get; set; }
        public bool PhoneAuthorized { get; set; }
        public int AuthorisedUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
