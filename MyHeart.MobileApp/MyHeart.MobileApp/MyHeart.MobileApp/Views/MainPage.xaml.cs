using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using MyHeart.MobileApp.Services;
using Xamarin.Forms;
using MyHeart.DTO.User;
using MyHeart.MobileApp.Utils.Helpers;
using System.Threading.Tasks;
using System.Linq;

namespace MyHeart.MobileApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
            RegisterServices();
            Children.Add(new MyReportsPage()
            {
                Title = "Dokumenty",
                IconImageSource = "documents.png"
            });
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
        }
    }
}