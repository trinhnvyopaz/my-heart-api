using System.ComponentModel.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHeart.DTO.MedicalReport;

namespace MyHeart.Data.Models
{
    public class ProcessedTextContent : BaseModel
    {
        public MedicalReportType Type { get; set; }
        public MedicalReportKey Key { get; set; }
        public string Value { get; set; }

        public virtual OCRResult OCRResult { get; set; }
    }
}
