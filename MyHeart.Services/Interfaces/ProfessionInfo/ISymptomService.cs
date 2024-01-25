using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface ISymptomService
    {
        Task<IEnumerable<SymptomDTO>> SymptomsListAsync();
        Task<SymptomDTO> SymptomAsync(int symptomId);
        Task<SymptomDTO> UpdateSymptom(SymptomDTO symptom, string user);
        Task<SymptomDTO> AddSymptomAsync(SymptomDTO symptom, string user);
        Task<Dictionary<string, string>> ValidateSymptom(SymptomDTO symptom);
        Task<SymptomDTO> DeleteSymptom(int symptomId);
        Task<DataTableResponse<SymptomDTO>> GetSymptomsTable(DataTableRequest request);
        Task<DataTableResponse<SymptomDTO>> GetPagedResource(SortedPagedSourceRequest request);
        Task<IEnumerable<SymptomDTO>> GetByIds(IEnumerable<int> ids);
    }
}