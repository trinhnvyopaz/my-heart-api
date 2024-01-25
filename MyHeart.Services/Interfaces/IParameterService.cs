using MyHeart.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyHeart.Services
{
    public interface IParameterService
    {
        Task<List<ParameterDTO>> GetListAsync();
        Task<ParameterDTO> CreateParameterAsync(ParameterDTO parameterDTO);
        Task<ParameterDTO> UpdateParameterAsync(ParameterDTO parameterDTO);
        Task<ParameterDTO> DeleteParameterAsync(int id);
        Task<List<ParameterDTO>> ExtractDataOCR(string ocrText);
    }
}
