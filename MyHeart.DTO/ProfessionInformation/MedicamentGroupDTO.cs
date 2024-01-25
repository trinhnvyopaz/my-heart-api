using MyHeart.DTO.ProfessionInformation.MedicamentGroupBonds;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class MedicamentGroupDTO : BaseEntityDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<MedicamentGroup_Disease_ContraindicationDTO> Contraindication { get; set; }
        public List<MedicamentGroup_Disease_IndicationDTO> Indication { get; set; }
        public List<MedicamentGroup_Pharmacy_ActiveSubstanceDTO> ActiveSubstance { get; set; }
        public List<MedicamentGroup_Symptoms_SideEffectsDTO> SideEffects { get; set; }
        public List<MedicamentGroup_AtcDTO> Atcs { get; set; }
        public List<PharmacyDTO> Pharmacies { get; set; }
    }
}
