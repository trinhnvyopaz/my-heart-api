using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MyHeart.Services.Interfaces;
using MyHeart.Services.Interfaces.Azure;
using MyHeart.Services.OpenData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Azure
{
    public class Blob : IBlob
    {
        private readonly IBlobConfiguration _blobConfig;
        private readonly BlobServiceClient _blobServiceClient;

        public Blob(IBlobConfiguration blobConfig)
        {
            _blobConfig = blobConfig;
            _blobServiceClient = new BlobServiceClient(_blobConfig.ConnectionString);
        }

        public async Task<bool> UploadFile(Stream fs, string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(
                    _blobConfig.PilBlob
                );
                var blobClient = containerClient.GetBlobClient(fileName);

                await blobClient.UploadAsync(fs, true);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> UploadFileWithContentType(
            Stream stream,
            string fileName,
            string contentType
        )
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(
                    _blobConfig.PilBlob
                );
                var blobClient = containerClient.GetBlobClient(fileName);

                await blobClient.UploadAsync(
                    stream,
                    new BlobHttpHeaders { ContentType = contentType }
                );
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<Stream> GetDownloadFileStream(string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(
                    _blobConfig.PilBlob
                );
                var blobClient = containerClient.GetBlobClient(fileName);

                var download = await blobClient.DownloadAsync();
                var str = new MemoryStream();
                await download.Value.Content.CopyToAsync(str);
                str.Position = 0;

                return str;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteFile(string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(
                    _blobConfig.PilBlob
                );
                var blobClient = containerClient.GetBlobClient(fileName);

                var res = await blobClient.DeleteAsync();
                return res.Status.ToString().StartsWith("2");
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteFiles(List<string> files)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(
                    _blobConfig.PilBlob
                );

                var tasks = new List<Task>();

                Parallel.ForEach(
                    files,
                    file =>
                    {
                        var blob = containerClient.GetBlobClient(file);
                        tasks.Add(blob.DeleteAsync());
                    }
                );

                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> UploadFiles(
            List<Tuple<string, string>> files,
            OpenDataPilZipHandler zip
        )
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(
                    _blobConfig.PilBlob
                );

                foreach (var file in files)
                {
                    var blob = containerClient.GetBlobClient(file.Item2);
                    await blob.UploadAsync(zip.GetStream(file.Item1), true);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<string> DownloadFile(string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(
                    _blobConfig.PilBlob
                );
                var blobClient = containerClient.GetBlobClient(fileName);

                var download = await blobClient.DownloadAsync();

                (new FileInfo(@$"dl/{fileName}")).Directory.Create();
                using (FileStream dlFS = File.OpenWrite(@$"dl/{fileName}"))
                {
                    await download.Value.Content.CopyToAsync(dlFS);
                    dlFS.Close();
                }

                return @$"dl/{fileName}";
            }
            catch (Exception e)
            {
                var a = fileName;
                var b = _blobConfig.PilBlob;
                System.Diagnostics.Debug.WriteLine(e);
                return null;
            }
        }
    }
}
