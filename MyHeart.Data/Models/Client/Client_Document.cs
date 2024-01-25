using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class Client_Document : BaseModel
    {
        [Required]
        public int ClientId { get; set; }
        public string FileUrl { get; set; }
        public DateTime UploadedDate { get; set; }
        public bool IsActive { get; set; }
        public virtual User Client { get; set; }
    }
}
