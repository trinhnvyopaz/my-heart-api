using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using Xamarin.Forms;
using MyHeart.MobileApp.Services;

namespace MyHeart.MobileApp.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            //await CrossMedia.Current.Initialize();
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.

            UIApplication.Main(args, nameof(MyHeartApplication), "AppDelegate");
        }
    }
    [Register(nameof(MyHeartApplication))]
    public class MyHeartApplication : UIApplication
    {
        private SessionService sessionService;
        public override void SendEvent(UIEvent uievent)
        {
            if (sessionService == null)
            {
                sessionService = DependencyService.Resolve<SessionService>();
            }

            base.SendEvent(uievent);
            var allTouches = uievent.AllTouches;
            if (allTouches != null && allTouches.Count > 0)
            {
                var phase = ((UITouch)allTouches.AnyObject).Phase;
                if (phase == UITouchPhase.Began || phase == UITouchPhase.Ended)
                {
                    sessionService.ResetSession();
                }
            }
        }
    }
}
