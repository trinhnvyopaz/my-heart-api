using MyHeart.DTO;
using MyHeart.Services.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services {
    public class SMSService : ISMSService {
        private readonly ISMSConfiguration _config;
        private readonly IEmailService _email;
        private RestClient Client => new RestClient("https://portal.bulkgate.com/api/1.0/simple/transactional");

        public SMSService(ISMSConfiguration configuration, IEmailService email) {
            _config = configuration;
            _email = email;
        }

        public async Task SendSMS(string message, string number) {
            var sms = new SMS() {
                application_id = _config.AppId,
                application_token = _config.AppToken,
                number = number,
                text = message,
                unicode = false,
                flash = false,
                sender_id = "gShort",
                sender_id_value = null,
                country = "cz"
            };

            var restRequest = new RestRequest();
            restRequest.AddJsonBody(sms);
            var result = await Client.ExecuteAsync(restRequest, Method.POST);

            if (!result.IsSuccessful) {
                var error = JsonConvert.DeserializeObject<SmsError>(result.Content);

                if (error.Type == "low_credit") {
                    await CriticalCredit();
                }
            }
        }

        private async Task CriticalCredit() {
            await _email.SendAdminMail("Kritický stav kreditu BULKGATE", "Máte kritický stav kreditu na www.bulkgate.com");
        }
    }
}
