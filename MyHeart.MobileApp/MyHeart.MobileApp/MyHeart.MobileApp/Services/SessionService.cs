using MyHeart.MobileApp.Views;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Services
{
    public class SessionService
    {
        private int maxInactivityDuration = 20;
        private readonly UserService userService;
        private readonly TimeSpan expirationTime;
        private bool IsActive { get; set; }
        private Timer sessionTimer;
        public SessionService()
        {
            userService = DependencyService.Resolve<UserService>();
            expirationTime = TimeSpan.FromMinutes(maxInactivityDuration);
        }

        public void StartSession()
        {
            IsActive = true;
            sessionTimer = new Timer(expirationTime.TotalMilliseconds);
            sessionTimer.Start();
            sessionTimer.Elapsed += SessionTimer_Elapsed;
        }
        public void ResetSession()
        {
            if (IsActive)
            {
                sessionTimer.Stop();
                sessionTimer.Start();
            }
        }
        private void SessionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                userService.User = null;
                userService.UserId = 0;

                NavigationCallBack.Callback = null;


                IPopupNavigation popupNavigation = PopupNavigation.Instance;
                popupNavigation.PopAllAsync();

                Application.Current.MainPage = new LoginPage();
            });
            StopSession();
        }

        public void StopSession()
        {
            IsActive = false;
            sessionTimer.Stop();
            sessionTimer.Elapsed -= SessionTimer_Elapsed;
        }
    }
}
