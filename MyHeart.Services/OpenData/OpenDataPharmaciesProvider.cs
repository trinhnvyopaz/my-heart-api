using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using MyHeart.DTO.ProfessionalInformation;

namespace MyHeart.Services.OpenData {
    class OpenDataPharmaciesProvider : IEnumerable<PharmacyDTO> {
        private readonly string path;
        StreamReader sr;

        public OpenDataPharmaciesProvider(string path) {
            this.path = path;
        }

        public IEnumerator<PharmacyDTO> GetEnumerator() {
            string line;

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (sr = new StreamReader(fs, encoding: System.Text.Encoding.GetEncoding(1250))) {
                sr.ReadLine();

                while ((line = sr.ReadLine()) != null) {
                    yield return PharmacyDTO.FromCsv(line);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
