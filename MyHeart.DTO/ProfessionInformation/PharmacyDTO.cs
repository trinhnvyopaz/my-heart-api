using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class PharmacyDTO : BaseEntityDTO
    {
        public List<Pharmacy_Symptoms_SideEffectsDTO> SideEffects { get; set; }
        public List<Pharmacy_Disease_ContraIndicationDTO> ContraIndication { get; set; }
        public List<Pharmacy_CommercialNamesDTO> CommercialNames { get; set; }
        public List<MedicamentGroup_Pharmacy_ActiveSubstanceDTO> ActiveSubstance { get; set; }
        public List<Pharmacy_Disease_IndicationDTO> Indication { get; set; }


        public string Dosage { get; set; }
        public string MinimumDose { get; set; }
        public string MaximumDose { get; set; }

        public int? SuklCode { get; set; }
        [Required(ErrorMessage ="Název je povinné pole")]
        public string Name { get; set; }
        public string Strength { get; set; }
        public string Form { get; set; }
        public string Package { get; set; }
        public string Path { get; set; }
        public string Supplement { get; set; }
        public string AtcWho { get; set; }
        public double? DddamntWho { get; set; }
        public string DddunWho { get; set; }
        [Required(ErrorMessage = "Název reg je povinné pole")]
        public string NameReg { get; set; }

        public static PharmacyDTO FromCsv(string csv) {
            var values = csv.Split(';');
            
            var med = new PharmacyDTO {
                SuklCode = int.Parse(values[0]),
                Name = values[2],
                Strength = values[3],
                Form = values[4],
                Package = values[5],
                Supplement = values[7],
                AtcWho = values[18],
                DddamntWho = string.IsNullOrEmpty(values[24]) ? 0.0 : double.Parse(values[24]),
                DddunWho = values[25],
                NameReg = values[38],
                Dosage = "INITIAL",
                MinimumDose = "INITIAL",
                MaximumDose = "INITIAL"
        };

            med.Id = med.SuklCode.Value;

            return med;
        }

        public override bool Equals(object obj) {
            return obj is PharmacyDTO dTO &&
                   this.Id == dTO.Id &&
                   this.LastUpdateDate == dTO.LastUpdateDate &&
                   this.LastUpdateUser == dTO.LastUpdateUser &&
                   this.Dosage == dTO.Dosage &&
                   this.MinimumDose == dTO.MinimumDose &&
                   this.MaximumDose == dTO.MaximumDose &&
                   this.SuklCode == dTO.SuklCode &&
                   this.Name == dTO.Name &&
                   this.Strength == dTO.Strength &&
                   this.Form == dTO.Form &&
                   this.Package == dTO.Package &&
                   this.Path == dTO.Path &&
                   this.Supplement == dTO.Supplement &&
                   this.AtcWho == dTO.AtcWho &&
                   this.DddamntWho == dTO.DddamntWho &&
                   this.DddunWho == dTO.DddunWho &&
                   this.NameReg == dTO.NameReg;
        }

        public bool EqualsWithoutBaseEntity(object obj) {
            return obj is PharmacyDTO dTO &&
                   this.Dosage == dTO.Dosage &&
                   this.MinimumDose == dTO.MinimumDose &&
                   this.MaximumDose == dTO.MaximumDose &&
                   this.SuklCode == dTO.SuklCode &&
                   this.Name == dTO.Name &&
                   this.Strength == dTO.Strength &&
                   this.Form == dTO.Form &&
                   this.Package == dTO.Package &&
                   this.Path == dTO.Path &&
                   this.Supplement == dTO.Supplement &&
                   this.AtcWho == dTO.AtcWho &&
                   this.DddamntWho == dTO.DddamntWho &&
                   this.DddunWho == dTO.DddunWho &&
                   this.NameReg == dTO.NameReg;
        }
    }
}
