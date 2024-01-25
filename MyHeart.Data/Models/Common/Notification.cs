using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.Models
{
    public class Notification : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? DiseaseId { get; set; }
        public Disease Disease { get; set; }

    }
}
