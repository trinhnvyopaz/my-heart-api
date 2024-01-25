using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class EmailTemplate : BaseModel
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}