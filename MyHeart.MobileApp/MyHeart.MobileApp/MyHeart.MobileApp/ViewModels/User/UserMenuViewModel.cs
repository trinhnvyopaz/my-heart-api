using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.Views;
using MyHeart.MobileApp.Views.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels.User
{
    public class UserMenuViewModel : BaseViewModel
    {
        public ICommand GoToNewsCommand { get; set; }
        public ICommand GoToNotificationsCommand { get; set; }
        public ICommand GotoUserPageCommand { get; set; }
        public ICommand GoBackCommand { get; set; }
        public ICommand LogoutCommand { get; set; }

        private SessionService sessionService;
        private UserService userService;
        private UserDTO user;

        public UserDTO User
        {
            get => user;
            set => SetProperty(ref user, value);
        }

        public UserMenuViewModel()
        {
            GoToNewsCommand = new Command(GoToNews);
            GoToNotificationsCommand = new Command(GoToNotifications);
            GotoUserPageCommand = new Command(GotoUserPage);
            GoBackCommand = new Command(GoBack);
            LogoutCommand = new Command(Logout);

            sessionService = DependencyService.Resolve<SessionService>();
            userService = DependencyService.Resolve<UserService>();

            _ = LoadData();
        }

        private void Logout()
        {
            sessionService.StopSession();
            SecureStorage.Remove("username");
            SecureStorage.Remove("password");
            SecureStorage.Remove("biometricAutheticationEnabled");
            SecureStorage.Remove("mfasecret");

            userService.User = null;
            userService.UserId = 0;

            NavigationCallBack.Callback = null;

            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }

        //CAU
        private async void GoBack()
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }

        private async void GotoUserPage()
        {
            await App.Current.MainPage.Navigation.PushAsync(new ProfilePage());
        }

        private async Task LoadData()
        {
            var user = await userService.GetCurrentAsync();
            if (user != null)
            {
                User = user;
            }
            else
            {
                toastService.ShowError("Nepodařilo se načíst uživatele");
            }
        }

        private async void GoToNotifications()
        {
            await App.Current.MainPage.Navigation.PushAsync(new UserNotificationPage());
        }

        private async void GoToNews()
        {
            //throw new NotImplementedException();
        }
    }
}
