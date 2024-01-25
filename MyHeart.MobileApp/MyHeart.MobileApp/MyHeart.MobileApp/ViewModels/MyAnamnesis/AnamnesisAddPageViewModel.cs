using MyHeart.DTO;
using MyHeart.MobileApp.Services;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public abstract class AnamnesisAddPageViewModel<T> : BaseViewModel
    {
        protected IToastService toastService;
        protected UserService userService;
        protected IPopupNavigation popupNavigation = PopupNavigation.Instance;

        private ObservableCollection<T> items;
        private string searchQuery;

        public ICommand SearchCommand { get; }
        public ICommand AddItemCommand { get; set; }
        public ICommand GoBackCommand { get; set; }
        public ICommand LoadMoreCommand { get; }

        protected DataTableRequest request;
        protected bool loadMoreEnabled = true;

        protected const int DefaulPage = 1;
        protected const int DefaultPageSize = 10;
        protected const int QueryMinLength = 3;

        protected int itemsTotalCountDb;
        private string secondSearchQuery;

        double searchQueryInterval = 500;
        Timer searchQueryTimer;

        public string SearchQuery
        {
            get => searchQuery;
            set
            {
                if (SetProperty(ref searchQuery, value) && searchQueryTimer != null)
                {
                    if (searchQueryTimer.Enabled)
                    {
                        searchQueryTimer.Stop();
                        searchQueryTimer.Start();
                    }
                    else
                    {
                        searchQueryTimer.Enabled = true;
                        searchQueryTimer.Start();
                    }
                }

            }
        }
        public string SecondSearchQuery
        {
            get => secondSearchQuery;
            set => SetProperty(ref secondSearchQuery, value);
        }
        public ObservableCollection<T> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }
        public AnamnesisAddPageViewModel()
        {
            toastService = DependencyService.Resolve<IToastService>();
            userService = DependencyService.Resolve<UserService>();

            AddItemCommand = new Command<T>(item => AddItem(item));
            GoBackCommand = new Command(() => popupNavigation.PopAsync());
            LoadMoreCommand = new Command(() => LoadMore());

            SearchQuery = "";
            SecondSearchQuery = "";

            searchQueryTimer = new Timer(searchQueryInterval);
            searchQueryTimer.Elapsed += SearchQueryTimer_Elapsed;

        }

        private void SearchQueryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            searchQueryTimer.Stop();
            searchQueryTimer.Enabled = false;
            FilterItems();
        }

        public abstract Task LoadData();

        public abstract Task AddItem(T item);

        public abstract Task FilterItems();
        public abstract Task LoadMore();

    }
}
