using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Services
{
    public class PharmacyService : BaseService
    {
        private const string Endpoint = "Pharmacy/";

        public async Task<IEnumerable<PharmacyDTO>> GetAllPharmacies()
        {
            var response = await restService.SendRequest<IEnumerable<PharmacyDTO>>($"{Endpoint}", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<DataTableResponse<PharmacyDTO>> GetPharmacyTable(DataTableRequest request)
        {
            var response = await restService.SendRequest<DataTableResponse<PharmacyDTO>>($"{Endpoint}getDataTable", HttpMethod.Post, request);

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
