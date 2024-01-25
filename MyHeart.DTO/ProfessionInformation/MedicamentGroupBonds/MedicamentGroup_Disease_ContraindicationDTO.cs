using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class MedicamentGroup_Disease_ContraindicationDTO : BaseEntityDTO
    {
        public int MedicamentGroupId { get; set; }
        public int DiseaseId { get; set; }

    }
}
