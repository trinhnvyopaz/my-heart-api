using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class SymptomQuestions_Symptom : BaseModel
    {
        [Required]
        public int SymptomQuestionsId { get; set; }
        [Required]
        public int SymptomsId { get; set; }
        [Required]
        public int BondStrength { get; set; }
        public virtual Symptoms Symptoms { get; set; }
        public virtual SymptomQuestions DiseaseQuestions { get; set; }
    }
}
