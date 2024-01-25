using System.ComponentModel.DataAnnotations;

namespace MyHeart.Data.Models
{
    public class UserReportFile : BaseModel
    {
        [Required]
        public int UserReportId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Extension { get; set; }
        [Required]
        public byte[] Content { get; set; }
        public string MimeType { get; set; }
        public UserReport UserReport { get; set; }
        public bool IsOCRProcessed { get; set; }

    }
}
