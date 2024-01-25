using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User {
    public class PhoneAuthDTO {
        public Guid Secret { get; set; }
        public int Id { get; set; }
        public string Image { get; set; }
    }
}
