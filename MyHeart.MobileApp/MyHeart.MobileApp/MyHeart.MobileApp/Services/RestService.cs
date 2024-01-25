using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.MobileApp.Services
{
    public class RestService
    {
        private HttpClient Client { get; set; }

        public RestService()
        {
            Client = ServiceConfig.Client;
        }
        public async Task<ApiResponse<T>> SendRequest<T>(string url, HttpMethod method, object data)
        {
            var response = await SendRequest(url, method, data);
            return new ApiResponse<T>(response);
        }
        public async Task<ApiResponse> SendRequest(string url, HttpMethod method, object data)
        {
            var apiResponse = new ApiResponse();

            try
            {
                HttpRequestMessage requestMessage = CreateResponseMessage(url, method, data);
                HttpResponseMessage response = null;

                response = await Client.SendAsync(requestMessage);

                apiResponse.StatusCode = response.StatusCode;
                apiResponse.Content = await response.Content.ReadAsStringAsync();
                apiResponse.IsSuccess = response.IsSuccessStatusCode;

                Debug.WriteLine($"RESPONSE: {response.StatusCode}, {apiResponse.Content}, {response.Headers}");
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
            }

            return apiResponse;
        }

        private HttpRequestMessage CreateResponseMessage(string url, HttpMethod method, object data)
        {
            var requestMessage = new HttpRequestMessage(method, url);

            if (data != null)
            {
                string jsonPayload = JsonConvert.SerializeObject(data);
                requestMessage.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            }

            return requestMessage;
        }
    }
    public class ApiResponse
    {
        public bool IsSuccess { get; set; } = false;

        public HttpStatusCode StatusCode { get; set; }
        public string Content { get; set; }
    }

    public class ApiResponse<TModel> : ApiResponse
    {
        public TModel Data { get; }

        public ApiResponse(ApiResponse response)
        {
            Content = response.Content;
            IsSuccess = response.IsSuccess;
            StatusCode = response.StatusCode;
            try
            {
                Data = JsonConvert.DeserializeObject<TModel>(Content);
            }
            catch (Exception ex)
            {
            }              
        }
    }
}
