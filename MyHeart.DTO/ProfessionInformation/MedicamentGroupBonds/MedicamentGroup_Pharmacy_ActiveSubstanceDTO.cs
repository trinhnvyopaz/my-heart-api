using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class MedicamentGroup_Pharmacy_ActiveSubstanceDTO : BaseEntityDTO
    {
        public int MedicamentGroupId { get; set; }
        public int PharmacyId { get; set; }
        public int BondStrength { get; set; }

    }
}
