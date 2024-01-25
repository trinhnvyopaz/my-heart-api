using MyHeart.Services.OpenData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces.Azure
{
    public interface IBlob
    {
        Task<bool> UploadFile(Stream fs, string fileName);
        Task<Stream> GetDownloadFileStream(string fileName);
        Task<bool> DeleteFile(string fileName);
        Task<string> DownloadFile(string fileName);
        Task<bool> DeleteFiles(List<string> files);
        Task<bool> UploadFiles(List<Tuple<string, string>> files, OpenDataPilZipHandler zip);

        Task<bool> UploadFileWithContentType(Stream fs, string fileName, string contentType);
    }
}
