using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Services.Interfaces {
    public interface IOpenDataConfiguration {
        public string BaseFolder { get; set; }
        public string StorageFolder { get; set; }
        public bool FirstFill { get; set; }
        public bool LimitImport { get; set; }
        public int LimitImportSize { get; set; }
        public bool SkipPill { get; set; }
    }
}
