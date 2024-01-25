using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO {
    public class SMS {
        public string application_id { get; set; }
        public string application_token { get; set; }
        public string number { get; set; }
        public string text { get; set; }
        public bool unicode { get; set; }
        public bool flash { get; set; }
        public string sender_id { get; set; }
        public string sender_id_value { get; set; }
        public string country { get; set; }
    }
}
