using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.ProfessionInformation
{
    public class MedicamentGroup_Pharmacy_ByAtc : BaseEntityDTO
    {
        public int MedicamentGroupId { get; set; }
        public int PharmacyId { get; set; }
        public string Name { get; set; }
        public bool Discarded { get; set; }
    }
}
