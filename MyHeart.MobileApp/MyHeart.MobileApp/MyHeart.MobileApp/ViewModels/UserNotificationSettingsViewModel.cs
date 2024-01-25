using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.ViewModels
{
    public class UserNotificationSettingsViewModel : BaseViewModel
    {
        private int userId;
        private bool therapyNewsEmailNotification;
        private bool replyEmailNotification;
        private int subscriptionPreferences;

        public int UserID 
        {
            get => userId; 
            set => SetProperty(ref userId, value); 
        }
        public bool TherapyNewsEmailNotification 
        { 
            get => therapyNewsEmailNotification; 
            set => SetProperty(ref therapyNewsEmailNotification, value); 
        }
        public bool ReplyEmailNotification 
        { 
            get => replyEmailNotification;
            set => SetProperty(ref replyEmailNotification, value); 
        }
        public int SubscriptionPreferences
        {
            get => subscriptionPreferences; 
            set => SetProperty(ref subscriptionPreferences, value); 
        }

        public UserNotificationSettingsViewModel(UserNotificationSettingsDTO dto)
        {
            UserID = dto.UserID;
            TherapyNewsEmailNotification = dto.TherapyNewsEmailNotification;
            ReplyEmailNotification = dto.ReplyEmailNotification;
            SubscriptionPreferences = dto.SubscriptionPreferences;
        }
    }
}
