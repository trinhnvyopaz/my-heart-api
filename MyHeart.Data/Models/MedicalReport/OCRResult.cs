using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHeart.Data.Models
{
    public class OCRResult : BaseModel
    {
        public string OCRText { get; set; }
        public int UserReportFilesId { get; set; }
        public List<ProcessedTextContent> ProcessedTextContents { get; set; }
    }
}
