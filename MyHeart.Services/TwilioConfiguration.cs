using System;
using System.Collections.Generic;
using System.Text;
using MyHeart.Services.Interfaces;

namespace MyHeart.Services
{
    public class TwilioConfiguration : ITwilioConfiguration
    {
        public string AccountSID { get; set; }
        public string AuthToken { get; set; }
        public string ApiKeySID { get; set; }
        public string ApiKeySecret { get; set; }
    }
}
