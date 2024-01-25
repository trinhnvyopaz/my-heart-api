using Foundation;
using MyHeart.MobileApp.iOS.PlatformServices;
using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(KeyboardServiceiOS))]
namespace MyHeart.MobileApp.iOS.PlatformServices
{
    public class KeyboardServiceiOS : IKeyboardService
    {
        public float Height { get; set; }

        public event EventHandler<KeyBoardSizeChangedEventArgs> OnKeyBoardSizeChanged;

        public KeyboardServiceiOS()
        {
            UIKeyboard.Notifications.ObserveWillShow((sender, args) =>
            {
                Height = (float)args.FrameEnd.Height;
                OnKeyBoardSizeChanged?.Invoke(this, new KeyBoardSizeChangedEventArgs(Height));

            });

            UIKeyboard.Notifications.ObserveWillHide((sender, args) =>
            {
                Height = 0;
                OnKeyBoardSizeChanged?.Invoke(this, new KeyBoardSizeChangedEventArgs(Height));
            });
        }
    }
}