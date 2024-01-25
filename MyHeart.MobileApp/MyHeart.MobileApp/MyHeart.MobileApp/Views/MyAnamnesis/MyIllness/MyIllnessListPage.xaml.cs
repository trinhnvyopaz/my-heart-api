using MyHeart.MobileApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views.MyAnamnesis.MyIllness
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyIllnessListPage : ContentPage
    {
        public MyIllnessListPage(ObservableCollection<UserDiseaseViewModel> items)
        {
            BindingContext = new MyIllnessListViewModel(items);
            InitializeComponent();
        }
    }
}