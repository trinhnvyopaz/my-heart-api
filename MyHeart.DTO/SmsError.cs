using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO {
    public class SmsError {
        public string Type { get; set; }
        public int Code { get; set; }
        public string Error { get; set; }
        public string Detail { get; set; }
    }
}
