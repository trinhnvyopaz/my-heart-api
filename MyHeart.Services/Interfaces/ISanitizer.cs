using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Services.Interfaces {
    public interface ISanitizer {
        public string SanitizeHtml(string html);
    }
}
