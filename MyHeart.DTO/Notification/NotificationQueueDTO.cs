using MyHeart.DTO.User;
using System;
using System.Collections.Generic;

namespace MyHeart.DTO.Notification
{
    public class NotificationQueueDTO : BaseEntityDTO
    {
        public int UserId { get; set; }
        public UserFmcDTO User { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Sended { get; set; }
        public Dictionary<string, string> CustomData { get; set; }
    }
}
