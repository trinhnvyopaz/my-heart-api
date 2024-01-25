using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class DoctorDetail : BaseModel
    {
        [Required]
        public int UserId { get; set; }
        //TODO - dodělat aktivní hodiny, osobní data apd.
        public string Phone { get; set; }
        public string Description { get; set; }
        public string WorkPlace { get; set; }
    }
}