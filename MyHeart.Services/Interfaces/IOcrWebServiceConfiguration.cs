using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IOcrWebServiceConfiguration
    {
        string ApiUrl { get; set; }
        string LicenseCode { get; set; }
        string UserName { get; set; }
    }
}