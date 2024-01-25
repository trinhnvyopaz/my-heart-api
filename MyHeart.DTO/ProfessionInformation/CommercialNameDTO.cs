using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class CommercialNameDTO : BaseEntityDTO
    {
        [Required]
        public string Name { get; set; }
        public virtual ICollection<CommercialName_PharmacyDTO> Pharmacy { get; set; }

    }
}
