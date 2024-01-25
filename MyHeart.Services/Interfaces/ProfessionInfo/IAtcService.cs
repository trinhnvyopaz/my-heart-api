using MyHeart.DTO;
using MyHeart.DTO.ProfessionInformation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces.ProfessionInfo
{
    public interface IAtcService
    {
        Task<IEnumerable<AtcDTO>> GetAllAtcs();
        Task<IEnumerable<AtcDTO>> GetByIds(IEnumerable<int> ids);
        Task<DataTableResponse<AtcDTO>> GetPagedResource(SortedPagedSourceRequest request);
        Task<bool> AddOrUpdateAtc(AtcDTO atc, string user);

        Task<bool> BulkInsertOrUpdate(List<AtcDTO> atcList, string user);
        Task<bool> BulkDelete(List<AtcDTO> atcs);
    }
}
