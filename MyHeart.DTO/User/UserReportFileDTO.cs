using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User
{
    public class UserReportFileDTO : BaseEntityDTO
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public byte[] Content { get; set; }
        public string MimeType { get; set; }
        public UserReportDTO UserReport { get; set; }
    }
}
