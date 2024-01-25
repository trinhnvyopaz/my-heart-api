using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.Data.Models
{
    public class Symptom_Synonyms : SynonymBaseModel
    {
        [Required]
        public int SymptomId { get; set; }
        public Symptoms Symptom { get; set; }
    }
}
