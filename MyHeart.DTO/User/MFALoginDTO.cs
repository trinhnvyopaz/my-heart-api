using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.User {
    public class MFALoginDTO {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string MfaCode { get; set; }
        public bool RememberMe { get; set; }
    }
}
