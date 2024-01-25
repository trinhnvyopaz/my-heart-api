using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.Views;
using Xamarin.Essentials;
using Plugin.FirebasePushNotification;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.AppCenter.Crashes;
using System.Threading.Tasks;
using MyHeart.MobileApp.Views.User;
using Xamarin.CommunityToolkit.Exceptions;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using Newtonsoft.Json;
using System.Linq;

namespace MyHeart.MobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            RegisterServices();

            ServiceConfig.Init();

            DevExpress.XamarinForms.Charts.Initializer.Init();
            DevExpress.XamarinForms.CollectionView.Initializer.Init();
            DevExpress.XamarinForms.Editors.Initializer.Init();
            DevExpress.XamarinForms.DataForm.Initializer.Init();
            DevExpress.XamarinForms.Navigation.Initializer.Init();
            DevExpress.XamarinForms.Popup.Initializer.Init();

            //SecureStorage.Remove("mfasecret");
            CrossFirebasePushNotification.Current.OnTokenRefresh += Current_OnTokenRefresh;
            CrossFirebasePushNotification.Current.OnNotificationOpened += Current_OnNotificationOpened;

            MessagingCenter.Subscribe<object, Dictionary<string, object>>(this, "NotificationTappediOS", (sender, arg) => HandleNotificationTapiOS(arg));

            LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;

            MainPage = new NavigationPage(new LoginPage());
        }


        private void OnNotificationActionTapped(NotificationActionEventArgs e)
        {
            if (e.IsTapped)
            {
                var data = JsonConvert.DeserializeObject<IDictionary<string, object>>(e.Request.ReturningData);
                HandleNotificationTap(data);
            }
        }

        private void Current_OnNotificationOpened(object source, FirebasePushNotificationResponseEventArgs e)
        {
            HandleNotificationTap(e.Data);
        }

        private void HandleNotificationTap(IDictionary<string, object> data)
        {
            try
            {
                var userService = DependencyService.Resolve<UserService>();
                bool navigate = false;

                bool loginPageActive = MainPage.Navigation.NavigationStack.Last().GetType() == typeof(LoginPage);

                if (userService.User != null && !loginPageActive)
                {
                    navigate = true;
                }

                HandleNavigation(data, navigate);               
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void HandleNotificationTapiOS(Dictionary<string, object> data)
        {
            try
            {
                if (data.TryGetValue("IsBackground", out object IsBackground))
                {
                    var userService = DependencyService.Resolve<UserService>();
                    bool navigate = false;

                    if (DeviceInfo.Platform == DevicePlatform.iOS && userService.User != null)
                    {
                        navigate = true;
                    }

                    HandleNavigation(data, navigate);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void HandleNavigation(IDictionary<string, object> data, bool navigate)
        {
            var type = (string)data["Type"];

            if (type == NotificationTypeConstants.News)
            {
                var id = Convert.ToInt16(data["Id"]);
                if (navigate)
                {
                    _ = NavigateToNews(id);
                }
                else
                {
                    NavigationCallBack.Callback = async () => await NavigateToNews(id);
                }
            }
            if (type == NotificationTypeConstants.QuestionReply)
            {
                var id = Convert.ToInt16(data["Id"]);
                if (navigate)
                {
                    _ = NavigateToQuestion(id);
                }
                else
                {
                    NavigationCallBack.Callback = async () => await NavigateToQuestion(id);
                }
            }
        }


        private async Task NavigateToNews(int newsId)
        {
            var therapyNewsService = DependencyService.Resolve<TherapyNewsService>();
            var news = await therapyNewsService.GetTherapyNew(newsId);
            if (news != null)
            {
                if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    await Task.Delay(500);
                }

                await MainPage.Navigation.PushModalAsync(new NewsDetailPage(news));
            }
        }

        private async Task NavigateToQuestion(int questionId)
        {
            var therapyNewsService = DependencyService.Resolve<QuestionsService>();
            var question = await therapyNewsService.GetQuestion(questionId);
            if (question != null)
            {
                if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    await Task.Delay(500);
                }

                await MainPage.Navigation.PushModalAsync(new ChatPage(question));
            }
        }



        private async void Current_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
        {
            var userService = DependencyService.Resolve<UserService>();
            if (userService.UserId != 0)
            {
                await userService.AddFmcToken(e.Token);
            }

            Debug.WriteLine($"Token: {e.Token}");

        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            MessagingCenter.Send(this, "OnConfigurationChanged");
        }

        protected override void OnResume()
        {
        }

        private void RegisterServices()
        {
            DependencyService.Register<UserService>();
            DependencyService.Register<PharmacyService>();
            DependencyService.Register<AnamnesisService>();
            DependencyService.Register<TherapyNewsService>();
            DependencyService.Register<UserNonPharmacyService>();
            DependencyService.Register<QuestionsService>();
            DependencyService.Register<RestService>();
            DependencyService.Register<NonpharmacyService>();
            DependencyService.Register<LoginService>();
            DependencyService.Register<DiseaseService>();
            DependencyService.Register<SessionService>();
            DependencyService.Register<SymptomService>();
            DependencyService.Register<SymtomQuestionService>();
            DependencyService.Register<QuestionaireService>();
            DependencyService.Register<DoctorService>();
        }
    }

    public class NotificationTypeConstants
    {
        public const string News = "NEWS";
        public const string QuestionReply = "QUESTIONANSWER";
    }
    public static class NavigationCallBack
    {
        public static Func<Task> Callback { get; set; }
    }
}
