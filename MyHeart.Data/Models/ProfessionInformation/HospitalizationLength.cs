using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class HospitalizationLength : BaseModel
    {
        [Required]
        public string length { get; set; }
    }
}
