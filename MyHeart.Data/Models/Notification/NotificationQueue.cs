using System;

namespace MyHeart.Data.Models
{
    public class NotificationQueue : BaseModel
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Sended { get; set; }
        public string CustomData { get; set; }
        public string Error { get; set; }
    }
}
