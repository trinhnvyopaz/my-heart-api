using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.Doctor
{
    public class RegisterDoctorDTO
    {
        [Required(ErrorMessage = "Email je povinný")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Jméno je povinné")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Příjmení je povinné")]
        public string LastName { get; set; }
    }
}