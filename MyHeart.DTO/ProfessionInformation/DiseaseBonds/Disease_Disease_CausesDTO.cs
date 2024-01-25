using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class Disease_Disease_CausesDTO : BaseEntityDTO
    {
        public int DiseaseId { get; set; }
        public int CauseId { get; set; }
        public int BondStrength { get; set; }
    }
}
