using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class MedicalFacilities_PreventiveMeasures_PreventiveDTO : BaseEntityDTO
    {
        public int MedicalFacilitiesId { get; set; }
        public int PreventiveMeasureId { get; set; }
        public int BondStrength { get; set; }

    }
}
