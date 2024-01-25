using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public abstract class AnamnesisDetailPageViewModel<T> : BaseViewModel
    {
        protected IToastService toastService;
        protected IPopupNavigation popupNavigation = PopupNavigation.Instance;

        private T item;

        public T Item
        {
            get => item;
            set => SetProperty(ref item, value);
        }
        public ICommand GoBackCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteItemCommand { get; set; }
     
        public AnamnesisDetailPageViewModel(T item)
        {
            toastService = DependencyService.Resolve<IToastService>();

            Item = item;

            GoBackCommand = new Command(() => GoBack());
            SaveCommand = new Command(() => SaveItem());
            DeleteItemCommand = new Command(() => DeleteItem());
        }

        public virtual async Task GoBack()
        {
            await popupNavigation.PopAsync();
        }

        public abstract Task SaveItem();
        public virtual Task DeleteItem()
        { 
            return Task.CompletedTask;
        }
    }
}
