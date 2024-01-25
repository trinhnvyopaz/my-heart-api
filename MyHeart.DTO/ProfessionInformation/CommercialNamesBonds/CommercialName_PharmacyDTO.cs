using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class CommercialName_PharmacyDTO : BaseEntityDTO
    {
        public int CommercialNamesId { get; set; }
        public int PharmacyId { get; set; }
        public int BondStrength { get; set; }
    }
}
