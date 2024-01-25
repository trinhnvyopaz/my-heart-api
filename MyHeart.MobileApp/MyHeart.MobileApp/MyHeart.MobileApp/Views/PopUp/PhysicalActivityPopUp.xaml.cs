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
    public partial class PhysicalActivityPopUp : Rg.Plugins.Popup.Pages.PopupPage
    {
        public PhysicalActivityPopUp(PhysicalActivityPopUpViewModel vm)
        {
            vm.Title = "Fyzická aktivity";
            BindingContext = vm;
            InitializeComponent();
        }
    }
}