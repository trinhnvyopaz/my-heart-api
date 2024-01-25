using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyTreatmentsPage : ContentPage
    {
        public MyTreatmentsPage()
        {
            BindingContext = new MyTreatmentsCardViewModel();
            InitializeComponent();
        }
    }
}