using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.MobileApp.Services
{
    public class SymptomService : BaseService
    {
        public const string Endpoint = "Symptom/";

        public async Task<DataTableResponse<SymptomDTO>> GetSymptomsTable(DataTableRequest request)
        {
            var response = await restService.SendRequest<DataTableResponse<SymptomDTO>>($"{Endpoint}getDataTable", HttpMethod.Post, request);

            if(response.IsSuccess)
            {
                return response.Data;
            }

            else
            {
                return null;
            }
        }
    }
}
