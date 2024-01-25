using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class UserPersonalDisease : BaseModel
    {
        [Required]
        public string Name { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
    }
}
