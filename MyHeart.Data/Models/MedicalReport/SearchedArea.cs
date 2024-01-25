using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHeart.DTO.MedicalReport;

namespace MyHeart.Data.Models
{
    public class SearchedArea : BaseModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public SearchAreaType Type { get; set; }
        public virtual ICollection<Parameter> Parameters { get; set; }
    }
}
