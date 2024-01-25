using MyHeart.DTO.MedicalReport;
using System.Collections.Generic;

namespace MyHeart.Data.Models
{
    public class Parameter : BaseModel
    {
        public MedicalReportType Type { get; set; }
        public MedicalReportKey Key { get; set; }
        public int SearchType { get; set; }
        public string Keywords { get; set; }
        public string ResultFormat { get; set; }
        public string ResultCondition { get; set; }
        public ConformityType ConformityType { get; set; }
        public string ResultExclusion { get; set; }
        public virtual ICollection<Marker> Markers { get; set; }
    }
}
