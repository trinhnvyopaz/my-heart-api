using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class Symptoms : BaseModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Disease_Symptoms_Symptoms> Diseases { get; set; }

        public virtual ICollection<SymptomQuestions_Symptom> SymptomQuestions { get; set; }
        public ICollection<Symptom_Synonyms> Synonyms { get; set; }
    }
}
