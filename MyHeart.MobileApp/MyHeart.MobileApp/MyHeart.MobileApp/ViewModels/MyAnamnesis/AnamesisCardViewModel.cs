using MyHeart.MobileApp.Services;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public abstract class AnamesisCardViewModel<T> : BaseViewModel
    {
        private ObservableCollection<T> items;

        protected IToastService toastService;
        protected IPopupNavigation popupNavigation = PopupNavigation.Instance;

        private ObservableCollection<AnamnesisCardRow> rows;

        public ICommand RefreshCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand GoToAddItemCommand { get; set; }
        public ICommand GoToDetailCommand { get; set; }

        public ObservableCollection<T> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        public ObservableCollection<AnamnesisCardRow> Rows
        {
            get => rows;
            set => SetProperty(ref rows, value);
        }

        public AnamesisCardViewModel()
        {
            toastService = DependencyService.Get<IToastService>();

            Items = new ObservableCollection<T>();
            Rows = new ObservableCollection<AnamnesisCardRow>();

            RefreshCommand = new Command(() => LoadData());
            DeleteItemCommand = new Command<T>(item => DeleteItem(item));
            GoToAddItemCommand = new Command(() => GoToAddItem());
            GoToDetailCommand = new Command<int>(item => GoToAddItem());
        }


        public virtual Task DeleteItem(T item)
        {
            return Task.CompletedTask;
        }
        public abstract Task LoadData();
        public virtual Task GoToAddItem()
        {
            return Task.CompletedTask;
        }
        public virtual Task GoToDetail(int id)
        {
            return Task.CompletedTask;
        }
    }

    public class AnamnesisCardRow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
    }
}
