using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User {
    public class LoginResponseDTO {
        public string Token { get; set; }
        public string MfaSecret { get; set; }
    }
}
