using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class NonpharmaticTherapy_SynonymsDTO : SynonymBaseDTO
    {
        public int NonpharmaticTherapyId { get; set; }
        public NonpharmacyDTO NonpharmaticTherapy { get; set; }
    }
}
