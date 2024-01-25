using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.OpenData {
    public class OpenDataPilZipHandler {
        private readonly string zipPath;
        private ZipArchive archive;

        public OpenDataPilZipHandler(string zipPath) {
            this.zipPath = zipPath;
            archive = ZipFile.OpenRead(zipPath);
        }

        public Stream GetStream(string name) {
            var entry = archive.Entries.Where(x => x.Name == name).FirstOrDefault();

            if (entry != null) {
                return entry.Open();
            }

            return null;
        }

        public void Close(bool deleteUnzipped = true) {
            archive.Dispose();
        }
    }
}
