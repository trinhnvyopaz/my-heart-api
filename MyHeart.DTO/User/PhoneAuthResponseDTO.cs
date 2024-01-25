using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User {
    public class PhoneAuthResponseDTO {
        public Guid Secret { get; set; }
        public int AuthId { get; set; }
    }
}
