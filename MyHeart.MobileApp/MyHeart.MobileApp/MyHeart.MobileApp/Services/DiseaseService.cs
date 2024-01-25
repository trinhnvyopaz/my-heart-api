using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.MobileApp.Services
{
    public class DiseaseService : BaseService
    {
        public const string Endpoint = "Disease/";

        public async Task<DataTableResponse<DiseaseDTO>> GetDiseasesTable(DataTableRequest request)
        {
            var response = await restService.SendRequest<DataTableResponse<DiseaseDTO>>($"{Endpoint}getDataTable", HttpMethod.Post, request);

            if (response.IsSuccess)
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
