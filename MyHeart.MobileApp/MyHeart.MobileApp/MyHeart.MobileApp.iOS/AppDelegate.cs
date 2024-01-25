using Foundation;
using MyHeart.MobileApp.Services;
using System.Linq;
using UIKit;
using UserNotifications;
using Xamarin;
using Xamarin.Forms;
using CoreFoundation;
using System;
using Plugin.FirebasePushNotification;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Collections.Generic;
using Plugin.LocalNotification;
using CoreGraphics;
using Plugin.LocalNotification.Platforms;
using static CoreFoundation.DispatchSource;
using KeyboardOverlap.Forms.Plugin.iOSUnified;

namespace MyHeart.MobileApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUNUserNotificationCenterDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        private SessionService sessionService;
        SnackbarView snackbar;
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Rg.Plugins.Popup.Popup.Init();
            global::Xamarin.Forms.Forms.Init();

            LocalNotificationCenter.SetUserNotificationCenterDelegate();

            LoadApplication(new App());

            FirebasePushNotificationManager.Initialize(options, true);

            DevExpress.XamarinForms.Charts.iOS.Initializer.Init();
            DevExpress.XamarinForms.CollectionView.iOS.Initializer.Init();
            DevExpress.XamarinForms.Editors.iOS.Initializer.Init();
            DevExpress.XamarinForms.DataForm.iOS.Initializer.Init();
            DevExpress.XamarinForms.Popup.iOS.Initializer.Init();
            DevExpress.XamarinForms.Navigation.iOS.Initializer.Init();
            NativeMedia.Platform.Init(GetTopViewController);
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            Plugin.InputKit.Platforms.iOS.Config.Init();
            var success = base.FinishedLaunching(app, options);

            AppCenter.Start("f765daec-942f-4dd0-9f31-413daa38bcb1",
                   typeof(Analytics), typeof(Crashes));

            UNUserNotificationCenter.Current.Delegate = this;

            // Request notification permissions
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, (granted, error) =>
            {
                if (error == null && granted)
                {
                    DispatchQueue.MainQueue.DispatchAsync(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                }
            });

            sessionService = DependencyService.Resolve<SessionService>();

            //if (success)
            //{
            //    var allGesturesRecognizer = new AllGesturesRecognizer(delegate
            //    {
            //        sessionService.ResetSession();
            //    });

            //    Window.AddGestureRecognizer(allGesturesRecognizer);
            //}



            return success;
        }
         

        public override void WillEnterForeground(UIApplication uiApplication)
        {
            LocalNotificationCenter.ResetApplicationIconBadgeNumber(uiApplication);
            base.WillEnterForeground(uiApplication);
        }

        public UIViewController GetTopViewController()
        {
            var vc = UIApplication.SharedApplication.KeyWindow.RootViewController;

            if (vc is UINavigationController navController)
                vc = navController.ViewControllers.Last();

            return vc;
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);

        }

        private Dictionary<string, object> ConvertNSDictionaryToDictionary(NSDictionary nsDictionary)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            foreach (var item in nsDictionary)
            {
                dictionary[item.Key.ToString()] = item.Value.ToString();
            }

            return dictionary;
        }

        // To receive notifications in foregroung on iOS 9 and below.
        // To receive notifications in background in any iOS version
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // If you disable method swizzling, you'll need to call this method. 
            // This lets FCM track message delivery and analytics, which is performed
            // automatically with method swizzling enabled.

            //bug on ios OnNotificationOpened is not triggered, its trigering OnRecieve do i need to add key  to check if its from background
            // Make a mutable copy of userInfo to add a new key-value pair
            NSMutableDictionary mutableUserInfo = new NSMutableDictionary(userInfo);
            System.Console.WriteLine(application.ApplicationState);
            // Check application state and handle the notification tap accordingly
            if (application.ApplicationState == UIApplicationState.Active)
            {
                var data = ConvertNSDictionaryToDictionary(mutableUserInfo);
                data.Add("IsBackground", false);
                MessagingCenter.Instance.Send<object, Dictionary<string, object>>(this, "NotificationTappediOS", data);

            }
            else if (application.ApplicationState == UIApplicationState.Inactive || application.ApplicationState == UIApplicationState.Background)
            {
                System.Console.WriteLine("App in background");
                // The app was in the background and has come to the foreground after user's tap
                // Your code to handle the notification tap
                // Add a new key-value pair to the userInfo
                var data = ConvertNSDictionaryToDictionary(mutableUserInfo);
                data.Add("IsBackground", true);

                MessagingCenter.Instance.Send<object, Dictionary<string, object>>(this, "NotificationTappediOS", data);
            }


            FirebasePushNotificationManager.DidReceiveMessage(mutableUserInfo);
            // Do your magic to handle the notification data
            System.Console.WriteLine(userInfo);

            completionHandler(UIBackgroundFetchResult.NewData);
        }

        public UIViewController GetCurrentViewController()
        {
            UIViewController viewController = Window.RootViewController;

            while (viewController.PresentedViewController != null)
            {
                viewController = viewController.PresentedViewController;
            }

            return viewController;
        }


        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Handle the notification content and decide how to present it here...

            // Here, we're saying to present the notification alert and play the sound.
            completionHandler(UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Sound);
        }

    }

    class AllGesturesRecognizer : UIGestureRecognizer
    {
        public delegate void OnTouchesEnded();

        private OnTouchesEnded touchesEndedDelegate;

        public AllGesturesRecognizer(OnTouchesEnded touchesEnded)
        {
            this.touchesEndedDelegate = touchesEnded;
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            this.State = UIGestureRecognizerState.Failed;

            this.touchesEndedDelegate();

            base.TouchesEnded(touches, evt);
        }
    }
}
