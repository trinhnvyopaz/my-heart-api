using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.Models.ProfessionInformation {
    public class Pil : BaseModel{
        public int SuklCode { get; set; }
        public bool OnWeb { get; set; }
        public string FilePath { get; set; }
        public DateTime DataUpdated { get; set; }
    }
}
