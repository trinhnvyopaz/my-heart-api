using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyHeart.DTO.User
{
    public class UserReportDTO : BaseEntityDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime CreationDate { get; set; }
        public List<UserReportFileDTO> Files { get; set; }
        public ReportTypeDTO ReportType { get; set; }
    }
    public enum ReportTypeDTO
    {
        AmbulanceReport,
        Hospitalization,
        LabResult
    }
    public static class ReportTypeFunc
    {
        public static readonly (ReportTypeDTO, string)[] pairs = new (ReportTypeDTO, string)[]
        {
            (ReportTypeDTO.AmbulanceReport, "Ambulantní zpráva"),
            (ReportTypeDTO.Hospitalization, "Hospitalizace"),
            (ReportTypeDTO.LabResult, "Výsledek laboratoře")
        };

        public static string ToString(ReportTypeDTO type)
        {
            (ReportTypeDTO, string)? res = pairs.First(pair => pair.Item1 == type);
            if (!res.HasValue)
            {
                throw new NotImplementedException();
            }
            return res.Value.Item2;
        }
        public static ReportTypeDTO ToReportType(string s)
        {
            (ReportTypeDTO, string)? res = pairs.First(pair => pair.Item2 == s);
            if (!res.HasValue)
            {
                throw new NotImplementedException();
            }
            return res.Value.Item1;
        }
    }
}
