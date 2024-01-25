using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHeart.Core.Helpers
{
    public class SecuritySettings
    {
        public string TokenSecret { get; set; }
        public int TokenLifetime { get; set; }
        public string Pepper { get; set; }
    }
}
