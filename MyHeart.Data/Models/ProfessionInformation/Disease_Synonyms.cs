using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.Data.Models.ProfessionInformation
{
    public class Disease_Synonyms : SynonymBaseModel
    {
        [Required]
        public int DiseaseId { get; set; }
        public Disease Disease { get; set; }
    }
}
