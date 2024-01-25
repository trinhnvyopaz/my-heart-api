using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.Models.ProfessionInformation
{
    public class PreventiveMeasures_Synonyms : SynonymBaseModel
    {
        public int PreventiveMeasureId { get; set; }
        public PreventiveMeasures PreventiveMeasure { get; set; }
    }
}
