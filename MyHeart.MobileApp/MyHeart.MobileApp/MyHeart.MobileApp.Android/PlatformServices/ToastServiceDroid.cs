using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using MyHeart.MobileApp.Droid.PlatformServices;
using MyHeart.MobileApp.Enums;
using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Android.App.ActionBar;

[assembly: Xamarin.Forms.Dependency(typeof(ToastServiceDroid))]
namespace MyHeart.MobileApp.Droid.PlatformServices
{
    public class ToastServiceDroid : IToastService
    {
        private Snackbar snackbar;
        private Color ErrorColor = (Color)Xamarin.Forms.Application.Current.Resources["ErrorColor"];
        private Color SuccessColor = (Color)Xamarin.Forms.Application.Current.Resources["SuccessColor"];

        private Snackbar CreateSnackBar(Color color, string message, NotificationPosition position)
        {
            var activity = Platform.CurrentActivity;
            Android.Views.View view = activity.FindViewById(Android.Resource.Id.Content);
            var snackBar = Snackbar.Make(view, message, Snackbar.LengthShort);

            try
            {
                Android.Views.View snackBarView = snackbar.View;
                var parameters = new LayoutParams(snackBarView.LayoutParameters);
                parameters.Gravity = MapSharedPositionToGravity(position);
                snackBarView.LayoutParameters = parameters;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            snackBar.View.SetBackgroundColor(ColorExtensions.ToPlatformColor(color));

            return snackBar;
        }

        public void ShowError(string message, NotificationPosition position)
        { 
            var snackBar = CreateSnackBar(ErrorColor, message, position);
            snackBar.Show();
        }

        public void ShowSuccess(string message, NotificationPosition position)
        {
            var snackBar = CreateSnackBar(SuccessColor, message, position);
            snackBar.Show();
        }
        private GravityFlags MapSharedPositionToGravity(NotificationPosition position)
        {
            switch (position)
            {
                case NotificationPosition.Top:
                    return GravityFlags.Top;
                case NotificationPosition.Bottom:
                    return GravityFlags.Bottom;
                default:
                    return GravityFlags.Bottom;
            }
        }

    }
}