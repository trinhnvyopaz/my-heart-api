using MyHeart.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateNotification(NotificationDto model);
        Task<bool> SendNotification(int userId, string title, string body, Dictionary<string, string> customData);
    }
}
