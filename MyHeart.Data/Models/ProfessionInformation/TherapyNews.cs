using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class TherapyNews : BaseModel
    {
        [Required]
        public string Text { get; set; }
        public string Description { get; set; }
        [Required]
        public string WebLink { get; set; }
        public DateTime CreateDate { get; set; }
        public ICollection<TherapyNews_Disease_Sick> Diseases { get; set; }


    }
}
