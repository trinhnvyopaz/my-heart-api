using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IImportService
    {
        Task<bool> ImportData(IFormFile file);
    }
}