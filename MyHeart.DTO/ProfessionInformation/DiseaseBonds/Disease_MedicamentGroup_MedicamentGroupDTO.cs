using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class Disease_MedicamentGroup_MedicamentGroupDTO : BaseEntityDTO
    {

        public int DiseaseId { get; set; }
        public int MedicamentGroupId { get; set; }
        public int BondStrength { get; set; }
    }
}
