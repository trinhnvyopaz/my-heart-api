using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models 
{
    public class SymptomQuestions_Disease : BaseModel
    {
        [Required]
        public int SymptomsQuestionsId { get; set; }
        [Required]
        public int DiseaseId { get; set; }
        [Required]
        public int BondStrength { get; set; }
        public virtual Disease Disease { get; set; }
        [ForeignKey("SymptomsQuestionsId")]
        public virtual SymptomQuestions DiseaseQuestions { get; set; }
    }
}
