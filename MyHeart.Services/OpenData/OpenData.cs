using MyHeart.Services.Interfaces;
using MyHeart.Services.Interfaces.ProfessionInfo;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using MyHeart.DTO.ProfessionInformation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MyHeart.Services.Interfaces.Azure;
using MyHeart.DTO.ProfessionalInformation;
using MyHeart.Data;
using EFCore.BulkExtensions;

namespace MyHeart.Services.OpenData
{
    public class OpenData : IOpenData
    {
        private readonly IOpenDataConfiguration _config;
        private readonly IServiceProvider _services;
        private readonly IBlob _blob;

        public OpenData(IOpenDataConfiguration configuration, IServiceProvider services, IBlob blob)
        {
            _config = configuration;
            _services = services;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _blob = blob;
        }

        public async Task<bool> UpdateData()
        {
            System.Diagnostics.Debug.WriteLine("Opendata started");
            var scopeFactory = _services.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var pharmacyService = provider.GetRequiredService<IPharmacyService>();
                var atcService = provider.GetRequiredService<IAtcService>();
                var facilityService = provider.GetRequiredService<IMedicalFacilitieService>();
                var pilService = provider.GetRequiredService<IPilservice>();

                var baseFolder = Path.Combine(Directory.GetCurrentDirectory(), _config.BaseFolder);
                var downloader = new OpenDataDownloader(baseFolder);

                var folder = await downloader.GetMedicamentFolder();

                await ProcessPharmacy(folder, pharmacyService);

                await ProcessAtc(folder, atcService);

                var facilityCsv = await downloader.GetFacilityFile();

                await ProcessFacilites(facilityCsv, facilityService);

                await ProcessPills(folder, pilService, downloader);
            }

            System.Diagnostics.Debug.WriteLine("Opendata done");
            return true;
        }

        private async Task ProcessPills(string folder, IPilservice pilService, OpenDataDownloader downloader)
        {
            var pils = new OpenDataPilProvider(folder + "/dlp_nazvydokumentu.csv", Path.Combine(Directory.GetCurrentDirectory(), _config.StorageFolder));
            var oldPils = await GetPreviousPils();

            var addList = new List<PilDTO>();
            var changeList = new List<PilDTO>();
            var pathsToDelete = new List<string>();
            var i = 0;
            foreach (var facility in pils)
            {
                if (_config.LimitImport)
                {
                    if (++i > _config.LimitImportSize) break;
                    System.Diagnostics.Debug.WriteLine("pils - " + i);
                }

                if (oldPils.ContainsKey(facility.Id))
                {
                    var old = oldPils[facility.Id];

                    if (!old.EqualsWithoutBaseEntity(facility))
                    {
                        changeList.Add(facility);
                    }

                    oldPils.Remove(facility.Id);
                }
                else
                {
                    addList.Add(facility);
                }
            }

            pathsToDelete.AddRange(oldPils.Values.Select(x => x.FilePath));

            addList.AddRange(changeList);

            await pilService.BulkDelete(oldPils.Values.ToList());
            await pilService.BulkInsertOrUpdate(addList, "SYSTEM");

            foreach (var path in pathsToDelete)
            {
                await _blob.DeleteFile(path);
            }

            if (addList.Any())
            {
                var zipPath = await downloader.GetPilZip();
                var zip = new OpenDataPilZipHandler(zipPath);

                var toUpload = addList.Where(x => !x.OnWeb && !string.IsNullOrEmpty(x.GetFileName())).Select(x => Tuple.Create(x.GetFileName(), x.FilePath)).ToList();
                await _blob.UploadFiles(toUpload, zip);

                zip.Close();
            }

            await _blob.DeleteFile($"opendata/old/{OpenDataDownloader.FacilityFile}");
            await _blob.UploadFile(File.OpenRead(folder + "/dlp_nazvydokumentu.csv"), $"opendata/old/dlp_nazvydokumentu.csv");
        }

        private async Task<Dictionary<int, PilDTO>> GetPreviousPils()
        {
            var oldFacilityPath = await _blob.DownloadFile($"opendata/old/dlp_nazvydokumentu.csv");

            if (!string.IsNullOrEmpty(oldFacilityPath))
            {
                var oldPilsProvider = new OpenDataPilProvider(oldFacilityPath, Path.Combine(Directory.GetCurrentDirectory(), _config.StorageFolder));

                var dict = new Dictionary<int, PilDTO>();
                foreach (var pil in oldPilsProvider)
                {
                    dict.Add(pil.Id, pil);
                }

                return dict;
            }

            return new Dictionary<int, PilDTO>();
        }

        private async Task ProcessFacilites(string facilityPath, IMedicalFacilitieService facilityService)
        {
            var facilities = new OpenDataMedicalFacilitiesProvider(facilityPath);
            var oldFacilities = await GetPreviousFacilities();

            var facilityList = new List<MedicalFacilitiesDTO>();
            var i = 0;
            foreach (var facility in facilities)
            {
                if (_config.LimitImport)
                {
                    if (++i > _config.LimitImportSize) break;
                    System.Diagnostics.Debug.WriteLine("facilities - " + i);
                }

                if (oldFacilities.ContainsKey(facility.Id))
                {
                    var old = oldFacilities[facility.Id];

                    if (!old.EqualsWithoutBaseEntity(facility))
                    {
                        facilityList.Add(facility);
                    }

                    oldFacilities.Remove(facility.Id);
                }
                else
                {
                    facilityList.Add(facility);
                }
            }

            await facilityService.BulkDelete(oldFacilities.Values.ToList());
            await facilityService.BulkInsertOrUpdate(facilityList, "SYSTEM");

            await _blob.DeleteFile($"opendata/old/{OpenDataDownloader.FacilityFile}");
            await _blob.UploadFile(File.OpenRead(facilityPath), $"opendata/old/{OpenDataDownloader.FacilityFile}");
        }

        private async Task<Dictionary<int, MedicalFacilitiesDTO>> GetPreviousFacilities()
        {
            var oldFacilityPath = await _blob.DownloadFile($"opendata/old/{OpenDataDownloader.FacilityFile}");

            if (!string.IsNullOrEmpty(oldFacilityPath))
            {
                var oldFacilitiesProvider = new OpenDataMedicalFacilitiesProvider(oldFacilityPath);

                var dict = new Dictionary<int, MedicalFacilitiesDTO>();
                foreach (var facility in oldFacilitiesProvider)
                {
                    dict.Add(facility.Id, facility);
                }

                return dict;
            }

            return new Dictionary<int, MedicalFacilitiesDTO>();
        }

        private async Task ProcessAtc(string folder, IAtcService atcService)
        {
            var atcs = new OpenDataATCsProvider(folder + "/dlp_atc.csv");
            var oldAtcs = await GetPreviousAtc();

            var atcList = new List<AtcDTO>();
            var i = 0;
            foreach (var atc in atcs)
            {
                if (_config.LimitImport)
                {
                    if (++i > _config.LimitImportSize) break;
                    System.Diagnostics.Debug.WriteLine("atc - " + i);
                }

                if (oldAtcs.ContainsKey(atc.Id))
                {
                    var old = oldAtcs[atc.Id];

                    if (!old.EqualsWithoutBaseEntity(atc))
                    {
                        atcList.Add(atc);
                    }

                    oldAtcs.Remove(atc.Id);
                }
                else
                {
                    atcList.Add(atc);
                }
            }

            await atcService.BulkDelete(oldAtcs.Values.ToList());
            await atcService.BulkInsertOrUpdate(atcList, "SYSTEM");

            await _blob.DeleteFile("opendata/old/dlp_atc.csv");
            await _blob.UploadFile(File.OpenRead(folder + "/dlp_atc.csv"), "opendata/old/dlp_atc.csv");
        }

        private async Task<Dictionary<int, AtcDTO>> GetPreviousAtc()
        {
            var oldAtcPath = await _blob.DownloadFile("opendata/old/dlp_atc.csv");

            if (!string.IsNullOrEmpty(oldAtcPath))
            {
                var oldAtcProvider = new OpenDataATCsProvider(oldAtcPath);

                var dict = new Dictionary<int, AtcDTO>();
                foreach (var atc in oldAtcProvider)
                {
                    dict.Add(atc.Id, atc);
                }

                return dict;
            }

            return new Dictionary<int, AtcDTO>();
        }

        private async Task ProcessPharmacy(string folder, IPharmacyService pharmacyService)
        {
            var pharmacies = new OpenDataPharmaciesProvider(folder + "/dlp_lecivepripravky.csv");
            var oldPharmacies = await GetPreviousPharmacies();

            var i = 0;
            var pharmacyList = new List<PharmacyDTO>();
            foreach (var pharmacy in pharmacies)
            {
                if (_config.LimitImport)
                {
                    if (++i > _config.LimitImportSize) break;
                    System.Diagnostics.Debug.WriteLine("pharmacy - " + i);
                }

                if (oldPharmacies.ContainsKey(pharmacy.SuklCode.Value))
                {
                    var old = oldPharmacies[pharmacy.SuklCode.Value];

                    if (!old.EqualsWithoutBaseEntity(pharmacy))
                    {
                        pharmacyList.Add(pharmacy);
                    }

                    oldPharmacies.Remove(pharmacy.SuklCode.Value);
                }
                else
                {
                    pharmacyList.Add(pharmacy);
                }
            }

            await pharmacyService.BulkDelete(oldPharmacies.Values.ToList());
            await pharmacyService.BulkInsertOrUpdate(pharmacyList, "SYSTEM");

            await _blob.DeleteFile("opendata/old/dlp_lecivepripravky.csv");
            await _blob.UploadFile(File.OpenRead(folder + "/dlp_lecivepripravky.csv"), "opendata/old/dlp_lecivepripravky.csv");
        }

        private async Task<Dictionary<int, PharmacyDTO>> GetPreviousPharmacies()
        {
            var oldPharmacyPath = await _blob.DownloadFile("opendata/old/dlp_lecivepripravky.csv");

            if (!string.IsNullOrEmpty(oldPharmacyPath))
            {
                var oldPharmaciesProvider = new OpenDataPharmaciesProvider(oldPharmacyPath);

                var dict = new Dictionary<int, PharmacyDTO>();
                foreach (var pharmacy in oldPharmaciesProvider)
                {
                    dict.Add(pharmacy.SuklCode.Value, pharmacy);
                }

                return dict;
            }

            return new Dictionary<int, PharmacyDTO>();
        }
    }
}
