using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User {
    public class UserNotificationSettingsDTO {
        public int UserID { get; set; }
        public bool TherapyNewsEmailNotification { get; set; }
        public bool ReplyEmailNotification { get; set; }
        public int SubscriptionPreferences { get; set; }
    }
}
