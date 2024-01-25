using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.User
{
    public class PasswordRecoverDTO
    {
        [Required]
        public string ConfirmString { get; set; }

        [Required(ErrorMessage ="Heslo je povinné")]
        [MinLength(6, ErrorMessage = "Minimální délka hesla je 6 znaků")]
        [Compare( nameof(PasswordAgain),ErrorMessage = "Hesla se neshodují")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Potvrzení hesla je povinné")]        
        public string PasswordAgain { get; set; }
    }
}