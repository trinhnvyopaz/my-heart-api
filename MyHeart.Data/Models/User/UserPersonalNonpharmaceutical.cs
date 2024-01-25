using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHeart.Data.Models
{
    public class UserPersonalNonpharmaceutical : BaseModel
    {
        [Required]
        public string Name { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
    }
}
