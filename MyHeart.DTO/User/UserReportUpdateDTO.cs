using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User
{
    public class UserReportUpdateDTO
    {
        public int ReportId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public ReportTypeDTO ReportType { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
