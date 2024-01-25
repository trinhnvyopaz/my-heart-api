using Foundation;
using MyHeart.MobileApp.Enums;
using MyHeart.MobileApp.iOS.PlatformServices;
using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTGSnackBar;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(ToastServiceiOS))]
namespace MyHeart.MobileApp.iOS.PlatformServices
{
    public class ToastServiceiOS : IToastService
    {
        TimeSpan duration = TimeSpan.FromSeconds(2);
        private Color ErrorColor = (Color)Xamarin.Forms.Application.Current.Resources["ErrorColor"];
        private Color SuccessColor = (Color)Xamarin.Forms.Application.Current.Resources["SuccessColor"];
        public void ShowError(string message, NotificationPosition position)
        {
            var snackBar = new TTGSnackbar(message);
            snackBar.LocationType = MapNotificationPositionToLocation(position);
            snackBar.Duration = duration;
            snackBar.BackgroundColor = ColorExtensions.ToPlatformColor(ErrorColor);
            snackBar.Show();
        }

        public void ShowSuccess(string message, NotificationPosition position)
        {
            var snackBar = new TTGSnackbar(message);
            snackBar.LocationType = MapNotificationPositionToLocation(position);
            snackBar.Duration = duration;
            snackBar.BackgroundColor = ColorExtensions.ToPlatformColor(SuccessColor);
            snackBar.Show();
        }
        private TTGSnackbarLocation MapNotificationPositionToLocation(NotificationPosition position)
        {
            switch (position)
            {
                case NotificationPosition.Top:
                    return TTGSnackbarLocation.Top;
                case NotificationPosition.Bottom:
                    return TTGSnackbarLocation.Bottom;
                default:
                    return TTGSnackbarLocation.Bottom;
            }
        }
    }
}