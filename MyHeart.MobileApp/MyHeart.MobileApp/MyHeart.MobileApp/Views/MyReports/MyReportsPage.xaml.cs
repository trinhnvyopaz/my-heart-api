using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MyHeart.MobileApp.ViewModels;
using MyHeart.DTO.User;
using Xamarin.Forms.Internals;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.ViewModels.MyReports;
using MyHeart.MobileApp.Views.MyReports;

namespace MyHeart.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyReportsPage : ContentPage
    {
        public MyReportsPage()
        {
            InitializeComponent();
        }
    }
}