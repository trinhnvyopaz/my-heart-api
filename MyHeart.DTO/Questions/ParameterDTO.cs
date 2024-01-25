
using MyHeart.DTO.MedicalReport;
using System.Collections.Generic;

namespace MyHeart.DTO
{
    public class ParameterDTO : BaseEntityDTO
    {
        public MedicalReportType Type { get; set; }
        public MedicalReportKey Key { get; set; }
        public string Value { get; set; }
        public int SearchType { get; set; }
        public string Keywords { get; set; }
        public string Result { get; set; }
        public string ResultCondition { get; set; }
        public ConformityType ConformityType { get; set; }
        public string ResultExclusion { get; set; }
        public string BeforeMarkerReally { get; set; }
        public virtual ICollection<MarkerDTO> Markers { get; set; }

    }
    public class MarkerDTO : BaseEntityDTO
    {
        public int ParameterId { get; set; }
        public MakerGroup MakerGroup { get; set; }
        public MakerType Type { get; set; }
        public string Value { get; set; }
    }
}
