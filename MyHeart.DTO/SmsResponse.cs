using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO {
    public class SmsResponse {
        public string Status { get; set; }
        public string Sms_id { get; set; }
        public double Price { get; set; }
        public double Credit { get; set; }
        public string Number { get; set; }
    }
}
