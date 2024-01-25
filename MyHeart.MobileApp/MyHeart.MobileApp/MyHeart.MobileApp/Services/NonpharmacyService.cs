using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.MobileApp.Services
{
    public class NonpharmacyService : BaseService
    {
        public const string Endpoint = "Nonpharmacy/";
        public async Task<DataTableResponse<NonpharmacyDTO>> GetNonPharmacyTable(DataTableRequest request)
        {
            var response = await restService.SendRequest<DataTableResponse<NonpharmacyDTO>>($"{Endpoint}getDataTable", HttpMethod.Post, request);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<IEnumerable<NonpharmacyDTO>> GetNonPharmacies()
        {
            var response = await restService.SendRequest<IEnumerable<NonpharmacyDTO>>($"{Endpoint}", HttpMethod.Get, null);

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
