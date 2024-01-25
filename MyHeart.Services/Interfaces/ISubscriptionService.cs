using MyHeart.Data.Models;
using MyHeart.DTO;
using MyHeart.DTO.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<SubscriptionDTO>> GetSubscriptions();
        Task<SubscriptionDTO> GetSubscription(int id);
    }
}