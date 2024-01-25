using System.Collections.Generic;

namespace MyHeart.Data.Models
{
    public class PatientMedicalRecord : BaseModel
    {
        public int UserReportFileId { get; set; }
        public string url { get; set; }
        public string OCRText { get; set; }
        public bool IsDataManager { get; set; }
        public virtual UserReportFile UserReportFile { get; set; }
        public virtual ICollection<PatientMedicalExamination> PatientMedicalExaminations { get; set; }
    }
}
