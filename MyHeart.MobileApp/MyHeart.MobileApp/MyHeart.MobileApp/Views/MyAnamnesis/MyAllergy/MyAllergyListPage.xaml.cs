using MyHeart.DTO.User;
using MyHeart.MobileApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views.MyAnamnesis.MyAllergy
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyAllergyListPage : ContentPage
    {
        public MyAllergyListPage(ObservableCollection<UserAnamnesisDTO> allergies)
        {
            BindingContext = new MyAlergyListViewModel(allergies);
            InitializeComponent();
        }
    }
}