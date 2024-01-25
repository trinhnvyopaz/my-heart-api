using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class Disease_Symptoms_SymptomsDTO : BaseEntityDTO
    {
        public int DiseaseId { get; set; }
        public int SymptomsId { get; set; }
        public int BondStrength { get; set; }
    }
}
