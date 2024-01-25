using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace MyHeart.DTO.ProfessionInformation {
    public class PilDTO : BaseEntityDTO {
        public int SuklCode { get; set; }
        public bool OnWeb { get; set; }
        public string FilePath { get; set; }
        public DateTime DataUpdated { get; set; }

        public static PilDTO FromCsv(string csv, string storage) {
            var values = csv.Split(';');
            var pil = new PilDTO {
                SuklCode = int.Parse(values[0]),
                FilePath = string.IsNullOrEmpty(values[1]) ? null : "opendata/pil/" + values[1],
                OnWeb = IsURL(values[1]),
                DataUpdated = string.IsNullOrEmpty(values[2]) ? default : DateTime.ParseExact(values[2], "dd.MM.yyyy", CultureInfo.InvariantCulture)
            };

            pil.Id = pil.SuklCode;

            return pil;
        }

        private static bool IsURL(string url) {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        public string GetFileName() {
            if (string.IsNullOrEmpty(FilePath) || OnWeb) return "";

            return FilePath.Replace("opendata/pil/", "");
        }

        public bool EqualsWithoutBaseEntity(object obj) {
            return obj is PilDTO dTO &&
                   this.SuklCode == dTO.SuklCode &&
                   this.OnWeb == dTO.OnWeb &&
                   this.FilePath == dTO.FilePath &&
                   this.DataUpdated == dTO.DataUpdated;
        }

        public override bool Equals(object obj) {
            return obj is PilDTO dTO &&
                   this.Id == dTO.Id &&
                   this.LastUpdateDate == dTO.LastUpdateDate &&
                   this.LastUpdateUser == dTO.LastUpdateUser &&
                   this.SuklCode == dTO.SuklCode &&
                   this.OnWeb == dTO.OnWeb &&
                   this.FilePath == dTO.FilePath &&
                   this.DataUpdated == dTO.DataUpdated;
        }
    }
}
