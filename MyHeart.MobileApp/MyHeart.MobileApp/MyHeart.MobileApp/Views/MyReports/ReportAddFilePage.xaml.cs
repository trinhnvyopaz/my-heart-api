using MyHeart.MobileApp.ViewModels.MyReports;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views.MyReports
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportAddFilePage : ContentPage
    {
        public ReportAddFilePage()
        {
            BindingContext = new ReportAddFilePageViewModel(Navigation);
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}