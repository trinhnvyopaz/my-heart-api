using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class Client_Therapy : BaseModel
    {
        [Required]
        public int ClientId { get; set; }
        [Required]
        public int PharmacyId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }
        public virtual User Client { get; set; }
    }
}
