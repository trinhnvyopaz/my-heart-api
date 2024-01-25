using MyHeart.DTO.User;
using MyHeart.MobileApp.Models;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels.MyAnamnesis
{
    public abstract class MyAnamnesisListViewModel<T> : BaseViewModel
    {
        protected IPopupNavigation popupNavigation = PopupNavigation.Instance;
        private ObservableCollection<T> items;

        public ICommand AddItemCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand GoToDetailCommand { get; }


        public ObservableCollection<T> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        public MyAnamnesisListViewModel()
        {
            GoBackCommand = new Command(() => GoBack());
            GoToDetailCommand = new Command<T>(item => GoToDetail(item));
            AddItemCommand = new Command(() => GoToCreateNew());
        }

        protected abstract Task GoToDetail(T item);

        protected abstract Task GoToCreateNew();
        protected virtual async void GoBack()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
