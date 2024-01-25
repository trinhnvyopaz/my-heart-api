using MyHeart.DTO;
using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public class AddUserNonpharmacyPageViewModel : AnamnesisAddPageViewModel<UserNonpharmacyViewModel>
    {
        private UserNonPharmacyService userNonPharmacyService;
        private NonpharmacyService nonPharmacyService;
        public AddUserNonpharmacyPageViewModel()
        {
            userNonPharmacyService = DependencyService.Resolve<UserNonPharmacyService>();
            nonPharmacyService = DependencyService.Resolve<NonpharmacyService>();

            _ = LoadData();
        }
        public override async Task LoadData()
        {
            IsBusy = true;

            //request = new DataTableRequest
            //{
            //    Page = DefaulPage,
            //    PageSize = DefaultPageSize,
            //    Filter = ""
            //};

            var nonPharmacies = await nonPharmacyService.GetNonPharmacies();
            if (nonPharmacies != null)
            {
                //itemsTotalCountDb = nonPharmacies.TotalCount;
                Items = new ObservableCollection<UserNonpharmacyViewModel>(nonPharmacies.Select(p => new UserNonpharmacyViewModel(p)));
            }

            loadMoreEnabled = true;

            IsBusy = false;
        }
        public override async Task AddItem(UserNonpharmacyViewModel item)
        {
            IsBusy = true;

            item.UserId = userService.UserId;

            var userNonpharmacy = await userNonPharmacyService.SaveUserNonpharmacy(item);
            if (userNonpharmacy != null)
            {
                MessagingCenter.Instance.Send(this, "OnUserNonpharmacyCreated", userNonpharmacy);
                GoBackCommand.Execute(null);
                NotifyCreated();
            }
            else
            {
                NotifyCreateFailed();
            }

            IsBusy = false;
        }

        public override async Task FilterItems()
        {
            IsBusy = true;

            //request.Filter = SearchQuery;
            //request.Page = DefaulPage;

            var nonPharmacies = await userNonPharmacyService.GetNonPharmaticTherapyByName(SearchQuery);

            if (nonPharmacies != null)
            {
                //itemsTotalCountDb = nonPharmacyTable.TotalCount;
                Items = new ObservableCollection<UserNonpharmacyViewModel>(nonPharmacies.Select(d => new UserNonpharmacyViewModel(d)));
            }

            IsBusy = false;
        }

        public override async Task LoadMore()
        {
            if (loadMoreEnabled)
            {
                loadMoreEnabled = false;

                if (Items.Count < itemsTotalCountDb)
                {
                    request.Page = request.Page + 1;

                    var pharmacytable = await nonPharmacyService.GetNonPharmacyTable(request);
                    if (pharmacytable != null)
                    {
                        itemsTotalCountDb = pharmacytable.TotalCount;
                        foreach (var item in pharmacytable.Data.ToList())
                        {
                            Items.Add(new UserNonpharmacyViewModel(item));
                        }

                    }
                }

                loadMoreEnabled = true;
            }
        }
    }
}
