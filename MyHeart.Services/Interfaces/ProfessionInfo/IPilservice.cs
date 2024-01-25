using MyHeart.DTO;
using MyHeart.DTO.ProfessionInformation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces.ProfessionInfo {
    public interface IPilservice {
        Task<(bool changed, string oldPath)> AddOrUpdatePil(PilDTO pil, string user);
        Task<PilResponse> GetPillBySukl(int sukl);
        Task<(Stream file, string name)> GetFileStreamBySukl(int sukl);

        Task<bool> BulkInsertOrUpdate(List<PilDTO> pils, string user);
        Task<bool> BulkDelete(List<PilDTO> pils);
    }
}
