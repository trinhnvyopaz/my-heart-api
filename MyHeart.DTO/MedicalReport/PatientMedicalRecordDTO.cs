using MyHeart.DTO.User;
using System.Collections.Generic;
using System.ComponentModel;

namespace MyHeart.DTO.MedicalReport
{
    public class PatientMedicalRecordDTO : BaseEntityDTO
    {
        public int UserReportFileId { get; set; }
        public string url { get; set; }
        public string OCRText { get; set; }
        public bool IsDataManager { get; set; }
        public UserReportFileDTO UserReportFile { get; set; }
        public List<PatientMedicalExaminationDTO> PatientMedicalExaminations { get; set; }
        public UserReportDTO UserReport { get; set; }
        public OptionData Data { get; set; } = new OptionData();
        public string Title { get; set; }
    }

    public class OptionData
    {
        public List<PatientMedicalExaminationDTO> MessageType { get; set; } = new List<PatientMedicalExaminationDTO>();
        public List<PatientMedicalExaminationDTO> MessageDate { get; set; } = new List<PatientMedicalExaminationDTO>();
        public List<PatientMedicalExaminationDTO> PersonalData { get; set; } = new List<PatientMedicalExaminationDTO>();
        public List<PatientMedicalExaminationDTO> HealthStatus { get; set; } = new List<PatientMedicalExaminationDTO>();
        public List<PatientMedicalExaminationDTO> KnownData { get; set; } = new List<PatientMedicalExaminationDTO>();
        public List<PatientMedicalExaminationDTO> NewlyDiscoveredData { get; set; } = new List<PatientMedicalExaminationDTO>();
        public List<PatientMedicalExaminationDTO> Other { get; set; } = new List<PatientMedicalExaminationDTO>();
    }
    public enum MedicalReportType
    {
        [Description("Dokument")]
        MessageType = 1,
        [Description("Osobní data")]
        MessageDate,
        [Description("Známá data")]
        PersonalData,
        [Description("Zdravotní stav")]
        HealthStatus,
        [Description("Známá data")]
        KnownData,
        [Description("Nová data")]
        NewlyDiscoveredData,
        [Description("Other")]
        Other,
    }

    public enum MedicalReportKey
    {
        [Description("Typ zprávy")]
        MessageType = 1,
        [Description("Datum zprávy")]
        MessageDate,

        // PersonalData
        [Description("Pracoviště")]
        Workplace,
        [Description("Jméno")]
        Name,
        [Description("Příjmení")]
        Surname,
        [Description("Rodné číslo")]
        PersonalIdentificationNumber,
        [Description("Adresa")]
        Address,


        // HealthStatus
        [Description("Výška")]
        Height,
        [Description("Váha")]
        Weight,
        [Description("Tlak krve")]
        BloodPressure,
        [Description("Tepová frekvence")]
        Pulse,
        [Description("LDL")]
        LDL,

        // Known Data
        [Description("Známá onemocnění")]
        KnownIllness,
        [Description("Známá farmaka")]
        AWellKnownPharmacy,
        [Description("Známé výkony")]
        KnownPerformances,
        // New Data
        [Description("Nová onemocnění")]
        NewDiseases,
        [Description("Nová farmaka")]
        NewPharmacy,
        [Description("Nové výkony")]
        NewPerformances,
        [Description("Vysazená léčba")]
        DiscontinuedTreatment,
        Other,
    }

    public enum MakerType
    {
        MessageMarker = 1,
        TextMarker = 2,
        DosingMarkers = 3,
        DateMarkers = 4,
        MarkerOfNegativeChange = 5,
        MarkerOfPositiveChange = 6,
        PhysicalExaminationMarker = 7,
    }
    public enum MakerGroup
    {
        Before,
        After,
    }

    public enum SearchAreaType
    {
        [Description("Prvních 1000 znaků textu")]
        First1000CharactersOfText = 1000,
        [Description("Prvních 500 znaků textu")]
        First500CharactersOfText = 500,
        [Description("100 znaků za markerem")]
        Characters100AfterTheMarker = 100,
        [Description("20 znaků za markerem")]
        Characters20AfterTheMarker = 20,
        [Description("Mezi markery před a za")]
        BetweenTheBeforeAndAfterMarkers = 0,
        [Description("6 znaků za markerem")]
        Characters6AfterTheMarker = 6,
    }

    public enum ConformityType
    {
        None = 0,
        [Description("Plná")]
        Full = 1,
        [Description("Částečná")]
        Partial = 2,
        [Description("Částečná, včetně synonym")]
        PartialAndIncludingSynonyms = 3,
    }
    public class MedicalReportExport
    {

        [MappingExcel("Mined item")]
        public string ItemMined { get; set; }

        [MappingExcel("Date for Mined item")]
        public string DateMined { get; set; }

        [MappingExcel("Note for Mined item")]
        public string NoteMined { get; set; }

        [MappingExcel("Dose for Mined Pharmacy")]
        public string DoseMined { get; set; }

        [MappingExcel("Dose unit for Mined Pharmacy")]
        public string DoseUnitMined { get; set; }

        [MappingExcel("Frequency of use for Mined Pharmacy")]
        public string FrequencyMined { get; set; }

        [MappingExcel("Data Set")]
        public string DataSet { get; set; }

        [MappingExcel("Data Class (= former specific data)")]
        public string DataClass { get; set; }

        [MappingExcel("Marker before - Source")]
        public string BeforeMarker { get; set; }

        [MappingExcel("Maker before Really used")]
        public string BeforeMarkerReally { get; set; }

        [MappingExcel("Area of search")]
        public string SearchArea { get; set; }

        [MappingExcel("Real locallization in Area of search")]
        public string SearchAreaReal { get; set; }

        [MappingExcel("Marker after - Source")]
        public string AfterMarker { get; set; }

        [MappingExcel("Marker after Really used")]
        public string AfterMarkerUsed { get; set; }

        [MappingExcel("Keyword Source")]
        public string KeywordSource { get; set; }

        [MappingExcel("Keyword Really used")]
        public string KeywordRelly { get; set; }

        [MappingExcel("Result format")]
        public string ResultFormat { get; set; }

        [MappingExcel("Result condition")]
        public string ResultCondition { get; set; }

        [MappingExcel("match")]
        public string Match { get; set; }

        [MappingExcel("Result - Exclusion condition")]
        public string ResultExclusion { get; set; }

        [MappingExcel("Result - Exclusion condition Really used")]
        public string ResultExclusionReally { get; set; }

        [MappingExcel("Path for saving data into User database")]
        public string PathSaving { get; set; }

        [MappingExcel("Date for Mined item - area after Mined item")]
        public string DateMinedArea { get; set; }

        [MappingExcel("Date for Mined item - real position after Mined item")]
        public string DateMinedReal { get; set; }

        [MappingExcel("Dosage for Mined Pharmacy - Source")]
        public string DoseMinedSource { get; set; }

        [MappingExcel("Dosage for Mined Pharmacy - Really used")]
        public string DoseMinedRelly { get; set; }
    }
}
