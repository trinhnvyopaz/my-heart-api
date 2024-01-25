using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class Pharmacy_CommercialNamesDTO : BaseEntityDTO
    {
        public string Name { get; set; }
        public int PharmacyId { get; set; }
    }
}
