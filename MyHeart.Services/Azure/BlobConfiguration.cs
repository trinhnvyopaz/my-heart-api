using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Services.Interfaces.Azure {
    public class BlobConfiguration : IBlobConfiguration {
        public string ConnectionString { get; set; }
        public string PilBlob { get; set; }
    }
}
