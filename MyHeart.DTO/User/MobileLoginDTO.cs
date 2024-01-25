using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User {
    public class MobileLoginDTO {
        public string Email { get; set; }
        public Guid Token { get; set; }
    }
}
