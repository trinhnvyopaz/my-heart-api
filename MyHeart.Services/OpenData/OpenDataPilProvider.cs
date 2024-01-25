using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using MyHeart.DTO.ProfessionInformation;

namespace MyHeart.Services.OpenData {
    class OpenDataPilProvider : IEnumerable<PilDTO> {
        private readonly string path;
        private readonly string storage;
        private StreamReader sr;

        public OpenDataPilProvider(string path, string storage) {
            this.path = path;
            this.storage = storage;
            (new FileInfo(storage)).Directory.Create();
        }

        public IEnumerator<PilDTO> GetEnumerator() {
            string line;

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (sr = new StreamReader(fs, encoding: System.Text.Encoding.GetEncoding(1250))) {
                sr.ReadLine();

                while ((line = sr.ReadLine()) != null) {
                    yield return PilDTO.FromCsv(line, storage);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
