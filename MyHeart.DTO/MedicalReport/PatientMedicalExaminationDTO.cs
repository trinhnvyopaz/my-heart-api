using System;

namespace MyHeart.DTO.MedicalReport
{
    public class PatientMedicalExaminationDTO : BaseEntityDTO
    {
        public MedicalReportType Type { get; set; }
        public MedicalReportKey Key { get; set; }
        //public string Value { get; set; }
        public bool IsSelected { get; set; }
        //public string Description { get; set; }
        public string KeyName => Enum.GetName(typeof(MedicalReportKey), Key);
        public int? DiseasePharmacyId { get; set; }
        public string ItemMined { get; set; }
        public string DateMined { get; set; }
        public string NoteMined { get; set; }
        public string DoseMined { get; set; }
        public string DoseUnitMined { get; set; }
        public string FrequencyMined { get; set; }
        public string BeforeMarkerReally { get; set; }
        public string SearchAreaReal { get; set; }
        public string AfterMarkerUsed { get; set; }
        public string KeywordRelly { get; set; }
        public string ResultExclusionReally { get; set; }
        public string PathSaving { get; set; }
        public string DateMinedArea { get; set; }
        public string DateMinedReal { get; set; }
        public string DoseMinedSource { get; set; }
        public string DoseMinedRelly { get; set; }
    }
}
