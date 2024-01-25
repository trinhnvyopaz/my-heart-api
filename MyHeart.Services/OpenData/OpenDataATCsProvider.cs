using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using MyHeart.DTO.ProfessionInformation;

namespace MyHeart.Services.OpenData {
    class OpenDataATCsProvider : IEnumerable<AtcDTO> {
        private readonly string path;
        StreamReader sr;

        public OpenDataATCsProvider(string path) {
            this.path = path;
        }

        public IEnumerator<AtcDTO> GetEnumerator() {
            string line;

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (sr = new StreamReader(fs, encoding: System.Text.Encoding.GetEncoding(1250))) {
                sr.ReadLine();

                while ((line = sr.ReadLine()) != null) {
                    yield return AtcDTO.FromCsv(line);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
