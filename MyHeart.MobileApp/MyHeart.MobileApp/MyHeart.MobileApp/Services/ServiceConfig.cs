using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MyHeart.MobileApp.Services
{
    public static class ServiceConfig
    {
        public static HttpClient Client;
        public static string authorizationToken;
        public const string autorizationScheme = "Bearer";
<<<<<<< HEAD
        public static string BaseURL => "https://myheartyopazapitest.azurewebsites.net/api/";
        public const string MFANeededCode = "MFANEEDED";
        // public const string BaseURL = "https://rmf1zlbb-61035.euw.devtunnels.ms/api/";
=======
        public static string BaseURL => "https://myheart-api-test.azurewebsites.net/api/";
        public const string MFANeededCode = "MFANEEDED";
        //public const string BaseURL = "https://rmf1zlbb-61035.euw.devtunnels.ms/api/";
>>>>>>> origin/master_customer_20231011
        public static void Init()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(BaseURL);
            Client.MaxResponseContentBufferSize = int.MaxValue;
            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            Client.Timeout = new TimeSpan(hours: 0, minutes: 0, seconds: 30);
        }

        public static void SetAuthToken(string token)
        {
            authorizationToken = token;
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(autorizationScheme, authorizationToken);
        }
    }
}
