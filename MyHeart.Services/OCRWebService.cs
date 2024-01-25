using System;
using System.IO;
using System.Net;
using System.Text;
using AutoMapper;
using MyHeart.Data;
using MyHeart.Services.Interfaces;
using Newtonsoft.Json;
using MyHeart.DTO.OCRWebService;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace MyHeart.Services
{
    public class OCRWebService : IOCRWebService
    {
        private readonly MyHeartContext _context;
        private readonly IMapper _mapper;

        private static IOcrWebServiceConfiguration _config;

        private readonly IHostingEnvironment _env;

        public OCRWebService(
            MyHeartContext context,
            IOcrWebServiceConfiguration configuration,
            IHostingEnvironment env
        )
        {
            _context = context;
            _config = configuration;
            _env = env;
        }

        public async Task<OCRResponseData> ProcessDocument(IFormFile file)
        {
            var filePath = await SaveUploadedFile(file);
            var result = await UploadAndRecognizeImage(filePath);
            return result;
        }

        private async Task<string> SaveUploadedFile(IFormFile file)
        {
            var savePath = Path.Combine(_env.WebRootPath, "Uploads", "OCR");
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(savePath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        private async Task<OCRResponseData> UploadAndRecognizeImage(string filePath)
        {
            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(_config.ApiUrl);
                byte[] authBytes = Encoding.UTF8.GetBytes(
                    string.Format("{0}:{1}", _config.UserName, _config.LicenseCode).ToCharArray()
                );
                webRequest.Method = "POST";
                webRequest.Timeout = 600000;
                webRequest.ContentType = "application/octet-stream";
                webRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(authBytes);

                using (var requestStream = await webRequest.GetRequestStreamAsync())
                {
                    using (var fileStream = System.IO.File.OpenRead(filePath))
                    {
                        await fileStream.CopyToAsync(requestStream);
                    }
                }

                var response = (HttpWebResponse)await webRequest.GetResponseAsync();
                var responseStream = response.GetResponseStream();

                using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    var result = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<OCRResponseData>(result);
                }
            }
            catch (System.Exception e)
            {
                throw;
            }
            finally
            {
                File.Delete(filePath);
            }
        }
    }
}
