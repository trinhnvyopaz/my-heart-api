using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.ProfessionInformation
{
    public class PreventiveMeasures_SynonymDTO : SynonymBaseDTO
    {
        public int PreventiveMeasureId { get; set; }
        public PreventiveMeasuresDTO PreventiveMeasure { get; set; }
    }
}
