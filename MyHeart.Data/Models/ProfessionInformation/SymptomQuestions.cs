using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class SymptomQuestions : BaseModel
    {
        [Required]
        public string Text { get; set; }
        public virtual ICollection<SymptomQuestions_Symptom> Symptoms { get; set; }
        public virtual ICollection<SymptomQuestions_Disease> Diseases { get; set; }
    }
}
