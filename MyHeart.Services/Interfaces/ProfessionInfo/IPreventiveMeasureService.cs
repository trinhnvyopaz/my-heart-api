using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IPreventiveMeasureService
    {
        Task<IEnumerable<PreventiveMeasuresDTO>> PreventiveMeasuresListAsync();
        Task<PreventiveMeasuresDTO> PreventiveMeasuresAsync(int preventiveMeasuresId);
        Task<PreventiveMeasuresDTO> UpdatePreventiveMeasures(PreventiveMeasuresDTO preventiveMeasures, string user);
        Task<PreventiveMeasuresDTO> AddPreventiveMeasuresAsync(PreventiveMeasuresDTO preventiveMeasures, string user);
        Task<PreventiveMeasuresDTO> DeletePreventiveMeasures(int preventiveMeasuresId);
        Task<Dictionary<string, string>> ValidatePreventiveMeasures(PreventiveMeasuresDTO symptom);
        Task<DataTableResponse<PreventiveMeasuresDTO>> GetPreventiveMeasuresTable(DataTableRequest request);
        Task<DataTableResponse<PreventiveMeasuresDTO>> GetPagedResource(SortedPagedSourceRequest request);
        Task<IEnumerable<PreventiveMeasuresDTO>> GetByIds(IEnumerable<int> ids);
    }
}