using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface ICommercialNameService
    {
        Task<IEnumerable<CommercialNameDTO>> CommercialNamesListAsync();
        Task<CommercialNameDTO> CommercialNamesAsync(int commercialNamesId);
        Task<CommercialNameDTO> UpdateCommercialNames(CommercialNameDTO commercialNames, string user);
        Task<CommercialNameDTO> AddCommercialNamesAsync(CommercialNameDTO commercialNames, string user);
        Task<CommercialNameDTO> DeleteCommercialNames(int commercialNamesId);
        Task<Dictionary<string, string>> ValidateCommercialNames(CommercialNameDTO commercialNames);
        
    }
}