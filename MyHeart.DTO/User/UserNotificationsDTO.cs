using MyHeart.DTO.Notification;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User
{
    public class UserNotificationsDTO
    {
        public int UserId { get; set; }
        public List<NotificationQueueDTO> Notifications { get; set; }
    }
}
