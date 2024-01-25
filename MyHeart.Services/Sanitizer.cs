using Ganss.XSS;
using MyHeart.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Services {
    public class Sanitizer : ISanitizer {
        private string[] _tagWhitelist = new string[] {
            "div",
            "b",
            "i",
            "u",
            "br",
            "td",
            "h1",
            "h2",
            "h3",
            "h4",
            "h5",
            "h6",
            "pre",
            "ol",
            "li",
            "ul",
            "table",
            "tbody",
            "tr",
            "td"
        };

        private string[] _attrWhitelist = new string[] {
            "align"
        };

        private HtmlSanitizer _sanitizer;

        public Sanitizer() {
            _sanitizer = new HtmlSanitizer(allowedTags: _tagWhitelist, allowedAttributes: _attrWhitelist);
        }

        public string SanitizeHtml(string html) {
            return _sanitizer.Sanitize(html);
        }
    }
}
