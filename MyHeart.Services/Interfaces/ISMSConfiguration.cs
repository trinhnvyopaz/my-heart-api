using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Services.Interfaces {
    public interface ISMSConfiguration {
        public string AppId { get; set; }
        public string AppToken { get; set; }
        public string AdminNumber { get; set; }
        public double LowCredit { get; set; }
    }
}
