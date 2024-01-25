using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface ITherapyNewsService
    {
        Task<IEnumerable<TherapyNewsDTO>> TherapyNewsListAsync();
        Task<TherapyNewsDTO> TherapyNewAsync(int therapyNewsId);
        Task<TherapyNewsDTO> UpdateTherapyNews(TherapyNewsDTO therapyNews, string user);
        Task<TherapyNewsDTO> AddTherapyNewsAsync(TherapyNewsDTO therapyNews, string user);
        Task<TherapyNewsDTO> DeleteTherapyNews(int therapyNewsId);
        Task<Dictionary<string, string>> ValidateTherapyNews(TherapyNewsDTO therapyNews);
        Task<DataTableResponse<TherapyNewsDTO>> GetTherapyNewsTable(DataTableRequest request);
        TherapyNewsSubscriptionSettingsDto GetSubscriptionPreferences();
        TherapyNewsSubscriptionSettingsDto GetSubscriptionPreferences(int id);
        Task<TherapyNewsSubscriptionSettingsDto> UpdateSubscriptionPreferences(TherapyNewsSubscriptionSettingsDto model, string user);
    }
}