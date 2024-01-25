using MyHeart.DTO.ProfessionalInformation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Services
{
    public class TherapyNewsService : BaseService
    {
        public const string Endpoint = "TherapyNews/";
        private UserService userService;

        public TherapyNewsService()
        {
            userService = DependencyService.Resolve<UserService>();
        }
        public async Task<TherapyNewsDTO> GetTherapyNew(int therapyNewsId)
        {
            var response = await restService.SendRequest<TherapyNewsDTO>($"{Endpoint}{therapyNewsId}", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<TherapyNewsDTO>> GetTherapyNews()
        {
            var response = await restService.SendRequest<IEnumerable<TherapyNewsDTO>>($"{Endpoint}", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<TherapyNewsSubscriptionSettingsDto> GetSubscriptionPreferences()
        {
            var response = await restService.SendRequest<TherapyNewsSubscriptionSettingsDto>($"{Endpoint}getSubscriptionPreferences", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<TherapyNewsSubscriptionSettingsDto> UpdateSubscriptionPreferences(int subscriptionPreferences, bool emailNotification)
        {
            var dto = new TherapyNewsSubscriptionSettingsDto
            {
                SubscriptionPreferences = subscriptionPreferences,
                TherapyNewsEmailNotification = emailNotification,
                UserId = userService.UserId
            };

            var response = await restService.SendRequest<TherapyNewsSubscriptionSettingsDto>($"{Endpoint}updateSubscriptionPreferences", HttpMethod.Post, dto);

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
