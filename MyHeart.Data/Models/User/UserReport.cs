using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace MyHeart.Data.Models
{
    public class UserReport : BaseModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public int UserId { get; set; }
        [Required]
        public DateTime LastUpdate { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public string Description { get; set; }

        public User User { get; set; }
        public List<UserReportFile> UserReportFiles { get; set; }
        [Required]
        public ReportType ReportType { get; set; }
    }
    public enum ReportType
    {
        AmbulanceReport,
        Hospitalization,
        LabResult
    }
}
