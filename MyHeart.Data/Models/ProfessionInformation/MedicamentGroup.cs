using MyHeart.Data.Models.ProfessionInformation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class MedicamentGroup : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<MedicamentGroup_Disease_Contraindication> Contraindication { get; set; }
        public ICollection<MedicamentGroup_Disease_Indication> Indication { get; set; }
        public ICollection<MedicamentGroup_Pharmacy_ActiveSubstance> ActiveSubstance { get; set; }
        public ICollection<MedicamentGroup_Symptoms_SideEffects> SideEffects { get; set; }
        public ICollection<MedicamentGroup_Atc> Atcs { get; set; }
        public ICollection<MedicamentGroup_Pharmacy_Discard> DiscardedPharmacies { get; set; }
    }
}
