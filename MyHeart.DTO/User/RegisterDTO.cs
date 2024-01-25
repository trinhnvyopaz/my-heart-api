using MyHeart.DTO.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.User
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Email je povinný")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Jméno je povinné")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Příjmení je povinné")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Heslo je povinné")]
        [MinLength(8, ErrorMessage = "Minimální délka hesla je 8 znaků")]
        [Compare(nameof(PasswordAgain), ErrorMessage = "Hesla se neshodují")]
        [ContainsUpperCase(ErrorMessage = "Heslo musí obsahovat alespoň jedno velké písmeno")]
        [ContainsLowerCase(ErrorMessage = "Heslo musí obsahovat alespoň jedno malé písmeno")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Ověření hesla je povinné")]
        [MinLength(6, ErrorMessage = "Minimální délka hesla je 8 znaků")]
        public string PasswordAgain { get; set; }
    }
}