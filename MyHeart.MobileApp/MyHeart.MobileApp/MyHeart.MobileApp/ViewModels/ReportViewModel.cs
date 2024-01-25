using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using MyHeart.DTO.User;

namespace MyHeart.MobileApp.ViewModels
{
    public class ReportViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime CreationDate { get; set; }
        public List<ReportFileViewModel> Files { get; set; }
        public UserReportDTO DTO { get; set; }
        public ReportTypeDTO ReportType { get; set; }
    }
    public static class DateFormatter
    {
        public static string GetDate(DateTime dt)
        {
            string s = $"{dt.Day}.{dt.Month}.{dt.Year}";
            return s;
        }
        public static string GetDateTime(DateTime dt)
        {
            return $"{dt.Day:00}.{dt.Month:00}.{dt.Year:0000} {dt.Hour:00}:{dt.Minute:00}";
        }

        internal static string GetTime(DateTime datetime)
        {
            // require 2 ciphers for each number
            return $"{datetime.Hour:00}:{datetime.Minute:00}:{datetime.Second:00}";
        }
    }

}
