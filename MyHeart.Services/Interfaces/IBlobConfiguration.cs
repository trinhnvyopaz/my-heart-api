using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Services.Interfaces {
    public interface IBlobConfiguration {
        public string ConnectionString { get; set; }
        public string PilBlob { get; set; }
    }
}
