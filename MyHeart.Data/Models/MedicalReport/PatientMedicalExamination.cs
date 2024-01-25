using MyHeart.DTO.MedicalReport;
using System.ComponentModel.DataAnnotations;

namespace MyHeart.Data.Models
{
    public class PatientMedicalExamination : BaseModel
    {
        public MedicalReportType Type { get; set; }
        public MedicalReportKey Key { get; set; }
        public virtual PatientMedicalRecord PatientMedicalRecord { get; set; }
        
        [Required]
        public bool IsSelected { get; set; }
        public int? DiseasePharmacyId { get; set; }
        public string ItemMined { get; set; }
        public string DateMined { get; set; }
        public string NoteMined { get; set; }
        public string DoseMined { get; set; }
        public string DoseUnitMined { get; set; }
        public string FrequencyMined { get; set; }
        public string BeforeMarkerReally { get; set; }
        public string SearchAreaReal { get; set; }
        public string AfterMarkerReally { get; set; }
        public string KeywordRelly { get; set; }
        public string ResultExclusionReally { get; set; }
        public string PathSaving { get; set; }
        public string DateMinedArea { get; set; }
        public string DateMinedReal { get; set; }
        public string DoseMinedSource { get; set; }
        public string DoseMinedRelly { get; set; }
    }
}
