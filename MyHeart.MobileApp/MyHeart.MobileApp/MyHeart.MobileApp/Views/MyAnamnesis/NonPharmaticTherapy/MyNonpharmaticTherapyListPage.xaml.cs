using MyHeart.MobileApp.ViewModels;
using MyHeart.MobileApp.ViewModels.MyAnamnesis.NonPharmaticTherapy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views.MyAnamnesis.NonPharmaticTherapy
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyNonpharmaticTherapyListPage : ContentPage
    {
        public MyNonpharmaticTherapyListPage(ObservableCollection<UserNonpharmacyViewModel> items)
        {
            BindingContext = new MyNonpharmaticTherapyListViewModel(items);
            InitializeComponent();
        }
    }
}