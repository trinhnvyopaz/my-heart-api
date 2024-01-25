using MyHeart.MobileApp.ViewModels;
using MyHeart.MobileApp.ViewModels.PopUps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views.PopUp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalAnamnesisPopUp : Rg.Plugins.Popup.Pages.PopupPage
    {
        public PersonalAnamnesisPopUp(PersonalAnamnesisPopUpViewModel vm)
        {
            BindingContext = vm;
            InitializeComponent();
        }
    }
}