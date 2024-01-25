using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO {
    public class PilResponse {
        public int Sukl { get; set; }
        public bool PilAvailable { get; set; }
        public bool PilOnWeb { get; set; }
        public string PilURL { get; set; }
        public string PilFileName { get; set; }
    }
}
