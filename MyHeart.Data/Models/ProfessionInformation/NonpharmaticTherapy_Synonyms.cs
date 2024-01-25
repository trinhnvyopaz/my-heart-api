using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.Models
{
    public class NonpharmaticTherapy_Synonyms : SynonymBaseModel
    {
        public int NonpharmaticTherapyId { get; set; }
        public NonpharmaticTherapy NonpharmaticTherapy { get; set; }
    }
}
