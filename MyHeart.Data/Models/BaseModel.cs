using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.Data.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }
        [Required]
        public DateTime LastUpdateDate { get; set; }
        [Required]
        public string LastUpdateUser { get; set; }
    }
}
