using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.ViewModels;
using MyHeart.MobileApp.ViewModels.MyReports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views.MyReports
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportAddInfoPage : ContentPage
    {

        public ReportAddInfoPage(ReportFileViewModel file)
        {
            InitializeComponent();
            BindingContext = new ReportAddInfoPageViewModel(file);
        }
    }
}