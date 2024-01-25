using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels.User
{
    public class NotificationSubscriptionPageViewModel : BaseViewModel
    {
        private UserNotificationSettingsViewModel notificationSettings;
        private UserService userService;

        public UserNotificationSettingsViewModel NotificationSettings
        {
            get => notificationSettings;
            set => SetProperty(ref notificationSettings, value);
        }

        public ICommand SaveNotificationSettingsCommand { get; set; }
        public ICommand GoBackCommand { get; set; }

        public NotificationSubscriptionPageViewModel()
        {
            userService = DependencyService.Resolve<UserService>();

            SaveNotificationSettingsCommand = new Command(() => SaveNotificationSettings());
            GoBackCommand = new Command(() => GoBack());

            _ = LoadData();
        }

        private async void GoBack()
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }

        public async Task LoadData()
        {
            var notificationResponse = await userService.GetNotificationSettings();
            if (notificationResponse != null)
            {
                NotificationSettings = new UserNotificationSettingsViewModel(notificationResponse);
            }
        }

        private async Task SaveNotificationSettings()
        {
            IsBusy = true;

            bool result = await userService.UpdateNotificationSettings(NotificationSettings);
            if (result)
            {
                toastService.ShowSuccess("Uloženo");
            }
            else
            {
                toastService.ShowError("Nepodařilo se uložit nastavení notifikací");
            }
            IsBusy = false;
        }
    }
}
