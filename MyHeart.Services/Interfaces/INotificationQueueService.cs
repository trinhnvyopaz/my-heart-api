using MyHeart.DTO;
using MyHeart.DTO.Notification;
using MyHeart.DTO.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface INotificationQueueService
    {
        Task<List<UserNotificationsDTO>> GetReadyToSendNotifications();
        Task UpdateSendedNotificationByIds(List<NotificationResult> ids);
        Task<List<NotificationQueueDTO>> GetNotificationsByUserId(int id);
        Task<List<NotificationQueueDTO>> CreateNotificationQueue(List<NotificationQueueDTO> notificationQueue);
    }
}