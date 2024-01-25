using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.Data.Models
{
    public class SynonymBaseModel : BaseModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public bool IsManual { get; set; }
    }
}
