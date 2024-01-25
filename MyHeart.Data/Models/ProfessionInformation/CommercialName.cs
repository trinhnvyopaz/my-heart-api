using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class CommercialName : BaseModel
    {
        [Required]
        public string Name { get; set; }
        public virtual ICollection<CommercialName_Pharmacy> Pharmacy { get; set; }

    }
}
