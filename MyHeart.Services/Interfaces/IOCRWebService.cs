using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MyHeart.DTO.OCRWebService;

namespace MyHeart.Services.Interfaces
{
    public interface IOCRWebService
    {
        public Task<OCRResponseData> ProcessDocument(IFormFile file);
    }
}
