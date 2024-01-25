using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class Pharmacy : BaseModel
    {
        public ICollection<Pharmacy_Symptoms_SideEffects> SideEffects { get; set; }
        public ICollection<Pharmacy_Disease_ContraIndication> ContraIndication { get; set; }
        public ICollection<Pharmacy_CommercialNames> CommercialNames { get; set; }
        public ICollection<MedicamentGroup_Pharmacy_ActiveSubstance> ActiveSubstance { get; set; }
        public ICollection<MedicamentGroup_Pharmacy_Discard> Discards { get; set; }
        public ICollection<Pharmacy_Disease_Indication> Indication { get; set; }

        public string Dosage { get; set; }
        public string MinimumDose { get; set; }
        public string MaximumDose { get; set; }

        public int? SuklCode { get; set; }
        public string Name { get; set; }
        public string Strength { get; set; }
        public string Form { get; set; }
        public string Package { get; set; }
        public string Path { get; set; }
        public string Supplement { get; set; }
        public string AtcWho { get; set; }
        public double? DddamntWho { get; set; }
        public string DddunWho { get; set; }
        public string NameReg { get; set; }

    }
}
