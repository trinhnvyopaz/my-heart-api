using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.OpenData {
    class OpenDataDownloader {
        public static readonly string FacilityFile = "facilities.csv";

        //https://opendata.sukl.cz/soubory/SOD20201030/DLP20201030.zip
        private string medicamentURL;
        private readonly string medicamentPageURL;
        private readonly string pilPageURL;
        private string pilURL;
        private string facilitiesURL;
        private readonly string facilitiesPageURL;
        private static readonly WebClient client = new WebClient();
        private int year;
        private int month;
        private readonly string folder;
        private readonly string DlpArchive;
        private readonly string PilArchive;
        private readonly string baseDir;
        private string medicamentArchiveReal;
        private string medicamentFolderURL;
        private string medicamentArchiveURL;
        private string medicamentFolderPath;
        private string pilArchiveReal;
        private string pilArchiveURL;
        private string facilityFile;
        private string pilZip;

        public OpenDataDownloader(string baseDir = null) {
            //"SOD", "DLP", "https://opendata.sukl.cz/soubory"
            medicamentURL = null;
            medicamentPageURL = "https://opendata.sukl.cz/?q=katalog/databaze-lecivych-pripravku-dlp";
            pilPageURL = "https://opendata.sukl.cz/?q=katalog/pil-pribalove-informace-product-information-leaflet";
            folder = "SOD";
            DlpArchive = "DLP";
            PilArchive = "PIL";

            //https://nrpzs.uzis.cz/res/file/export/export-2020-11.csv
            facilitiesURL = "https://nrpzs.uzis.cz/res/file/export/";
            facilitiesPageURL = "https://nrpzs.uzis.cz/index.php?pg=home--download";

            this.baseDir = string.IsNullOrEmpty(baseDir) ? Directory.GetCurrentDirectory() + "/downloads/" : baseDir;

            if (!Directory.Exists(baseDir)) {
                Directory.CreateDirectory(this.baseDir);
            }
        }

        public async Task<string> GetPilZip() {
            if (!string.IsNullOrEmpty(pilZip)) {
                return pilZip;
            }

            await GetmedicamentURL();
            await GetPilArchiveURL();
            DownloadPilArchive();

            return pilZip;
        }

        public async Task<string> GetMedicamentFolder() {
            if (!string.IsNullOrEmpty(medicamentFolderPath)) {
                return medicamentFolderPath;
            }

            await GetmedicamentURL();
            if (DownloadMedicamentArchive() == "downloaded") {
                ExtractMedicamentArhive();
            }

            return medicamentFolderPath;
        }

        public async Task<string> GetFacilityFile() {
            if (!string.IsNullOrEmpty(facilityFile)) {
                return facilityFile;
            }

            await DownloadFacilityCsv();

            return facilityFile;
        }

        private string DownloadPilArchive() {
            pilArchiveReal = (new Uri(pilArchiveURL)).Segments[^1];

            if (File.Exists(baseDir + pilArchiveReal)) {
                Console.WriteLine("folder found");
                pilZip = baseDir + pilArchiveReal;
                return "existed";
            }

            foreach (var d in Directory.GetFiles(baseDir)) {
                if (d.Split('\\')[^1].StartsWith(PilArchive)) {
                    File.Delete(d);
                }
            }

            if (File.Exists(baseDir + pilArchiveReal)) {
                File.Delete(baseDir + pilArchiveReal);
            }

            client.DownloadFile(new Uri(pilArchiveURL), baseDir + pilArchiveReal);
            pilZip = baseDir + pilArchiveReal;

            return "downloaded";
        }

        private async Task<string> DownloadFacilityCsv() {
            await GetFacilityCSVURL();
            var fileName = (new Uri("https://nrpzs.uzis.cz/" + facilitiesURL)).Segments[^1];

            if (File.Exists(baseDir + FacilityFile)) {
                facilityFile = baseDir + FacilityFile;
                return "existed";
            }

            foreach (var f in Directory.GetFiles(baseDir)) {
                if (f.Split('\\')[^1].StartsWith("export-")) {
                    File.Delete(f);
                }
            }


            client.DownloadFile(new Uri("https://nrpzs.uzis.cz/" + facilitiesURL), baseDir + FacilityFile);
            facilityFile = baseDir + FacilityFile;

            return "downloaded";
        }

        private async Task<bool> GetFacilityCSVURL() {
            var res = await client.DownloadStringTaskAsync(facilitiesPageURL);

            if (res != null) {
                var doc = new HtmlDocument();
                doc.LoadHtml(res);

                var workMonth = month;
                var workYear = year;
                IEnumerable<string> items;

                /*do {
                    System.Diagnostics.Debug.WriteLine($"trying medicalFolder: {workYear}:{workMonth}");
                    if (workMonth <= 0) {
                        workMonth = 12;
                        --workYear;
                    }
                    items = doc.DocumentNode.Descendants("a")
                    .Where(x => x.InnerText.ToLower().StartsWith($"{folder}{workYear}{workMonth}".ToLower()) && !x.InnerText.EndsWith("i/"))
                    .Select(y => y.InnerText);
                    
                    --workMonth;
                } while (!items.Any() && year > 2000);*/

                facilitiesURL = doc.DocumentNode.Descendants("a")
                    .Where(x => x.GetAttributeValue("href", "def").ToString().Contains("export"))
                    .First()
                    .Attributes
                    .Where(x => x.Name == "href")
                    .First()
                    .Value;

                return facilitiesURL != null;
            }

            return false;
        }

        private string DownloadMedicamentArchive() {
            medicamentArchiveReal = (new Uri(medicamentURL)).Segments[^1];
            
            if (Directory.Exists(baseDir + medicamentArchiveReal.Replace(".zip", ""))) {
                Console.WriteLine("folder found");
                medicamentFolderPath = baseDir + medicamentArchiveReal.Replace(".zip", "");
                return "existed";
            }

            foreach (var d in Directory.GetDirectories(baseDir)) {
                if (Path.GetDirectoryName(d).StartsWith(DlpArchive)) {
                    Directory.Delete(d, true);
                }
            }

            if (File.Exists(baseDir + medicamentArchiveReal)) {
                File.Delete(baseDir + medicamentArchiveReal);
            }

            client.DownloadFile(new Uri(medicamentURL), baseDir + medicamentArchiveReal);

            return "downloaded";
        }

        private void ExtractMedicamentArhive() {
            var dir = baseDir + medicamentArchiveReal.Replace(".zip", "");

            if (Directory.Exists(dir)) {
                foreach (var file in Directory.GetFiles(dir)) {
                    File.Delete(file);
                }
            }

            ZipFile.ExtractToDirectory(baseDir + medicamentArchiveReal, dir);
            File.Delete(baseDir + medicamentArchiveReal);
            Console.WriteLine("extract Done");
            medicamentFolderPath = dir;
        }

        private async Task<bool> GetmedicamentURL() {
            var res = await client.DownloadStringTaskAsync(medicamentPageURL);

            if (res != null) {
                var doc = new HtmlDocument();
                doc.LoadHtml(res);

                var workMonth = month;
                var workYear = year;
                IEnumerable<string> items;

                /*do {
                    System.Diagnostics.Debug.WriteLine($"trying medicalFolder: {workYear}:{workMonth}");
                    if (workMonth <= 0) {
                        workMonth = 12;
                        --workYear;
                    }
                    items = doc.DocumentNode.Descendants("a")
                    .Where(x => x.InnerText.ToLower().StartsWith($"{folder}{workYear}{workMonth}".ToLower()) && !x.InnerText.EndsWith("i/"))
                    .Select(y => y.InnerText);
                    
                    --workMonth;
                } while (!items.Any() && year > 2000);*/

                medicamentURL = doc.DocumentNode.Descendants("a")
                    .Where(x => x.InnerText.ToLower().EndsWith(".zip"))
                    .First()
                    .Attributes
                    .Where(x => x.Name == "href")
                    .First()
                    .Value;

                return medicamentFolderURL != null;
            }

            return false;
        }

        private async Task<bool> GetPilArchiveURL() {
            var res = await client.DownloadStringTaskAsync(pilPageURL);

            if (res != null) {
                var doc = new HtmlDocument();
                doc.LoadHtml(res);

                var workMonth = month;
                var workYear = year;
                IEnumerable<string> items;

                /*do {
                    System.Diagnostics.Debug.WriteLine($"trying medicalFolder: {workYear}:{workMonth}");
                    if (workMonth <= 0) {
                        workMonth = 12;
                        --workYear;
                    }
                    items = doc.DocumentNode.Descendants("a")
                    .Where(x => x.InnerText.ToLower().StartsWith($"{PilArchive}{workYear}{workMonth}".ToLower()) && !x.InnerText.EndsWith("i/"))
                    .Select(y => y.InnerText);

                    --workMonth;
                } while (!items.Any() && year > 2000);*/

                pilArchiveURL = doc.DocumentNode.Descendants("a")
                    .Where(x => x.InnerText.ToLower().EndsWith(".zip"))
                    .First()
                    .Attributes
                    .Where(x => x.Name == "href")
                    .First()
                    .Value;

                return pilArchiveURL != null;
            }

            return false;
        }
    }
}
