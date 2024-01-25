using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public abstract class BasePopUpViewModel : BaseViewModel
    {

        private string title;
        protected IPopupNavigation popupNavigation = PopupNavigation.Instance;

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public ICommand GoBackCommand { get; }
        public ICommand SaveCommand { get; }
        public BasePopUpViewModel()
        {
            GoBackCommand = new Command(() => popupNavigation.PopAsync());
            SaveCommand = new Command(() => SaveItem());
        }
       
        protected abstract Task SaveItem();
    }
}
