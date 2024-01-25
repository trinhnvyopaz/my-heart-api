using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public class OcrWebServiceConfiguration : IOcrWebServiceConfiguration
    {
        public string ApiUrl { get; set; }
        public string LicenseCode { get; set; }
        public string UserName { get; set; }
    }
}