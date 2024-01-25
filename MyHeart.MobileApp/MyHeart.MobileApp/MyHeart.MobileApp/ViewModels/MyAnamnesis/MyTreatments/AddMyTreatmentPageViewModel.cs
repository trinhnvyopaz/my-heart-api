using MyHeart.DTO;
using MyHeart.DTO.User;
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
    public class AddMyTreatmentPageViewModel : AnamnesisAddPageViewModel<UserAnamnesisViewModel>
    {
        private AnamnesisService anamnesisService;
        private PharmacyService pharmacyService;

        public ICommand ToggleDosageDetailCommand { get; }

        public AddMyTreatmentPageViewModel()
        {
            anamnesisService = DependencyService.Resolve<AnamnesisService>();
            pharmacyService = DependencyService.Resolve<PharmacyService>();

            ToggleDosageDetailCommand = new Command<UserAnamnesisViewModel>(userAnamnesis => userAnamnesis.ShowDosageDetail = !userAnamnesis.ShowDosageDetail);

            _ = LoadData();
        }

        public override async Task LoadMore()
        {
            if (loadMoreEnabled)
            {
                loadMoreEnabled = false;

                if (Items.Count < itemsTotalCountDb)
                {
                    request.Page = request.Page + 1;

                    var pharmacytable = await pharmacyService.GetPharmacyTable(request);
                    if (pharmacytable != null)
                    {
                        itemsTotalCountDb = pharmacytable.TotalCount;
                        var openedDetails = Items.Where(i => i.ShowDosageDetail).ToList();
                        foreach (var detail in openedDetails)
                        {
                            detail.ShowDosageDetail = false;
                        }

                        foreach (var item in pharmacytable.Data.ToList())
                        {
                            Items.Add(new UserAnamnesisViewModel(item)
                            {
                                CreatedAt = DateTime.Now
                            });
                        }

                        foreach (var detail in openedDetails)
                        {
                            detail.ShowDosageDetail = true;
                        }
                    }
                }

                loadMoreEnabled = true;
            }
        }

        public override async Task LoadData()
        {
            request = new DataTableRequest
            {
                Page = DefaulPage,
                PageSize = DefaultPageSize,
                Filter = "",
                SecondFilter = ""
            };

            IsBusy = true;

            var pharmacytable = await pharmacyService.GetPharmacyTable(request);
            if (pharmacytable != null)
            {
                itemsTotalCountDb = pharmacytable.TotalCount;
                Items = new ObservableCollection<UserAnamnesisViewModel>(pharmacytable.Data.Select(d => new UserAnamnesisViewModel(d)
                {
                    CreatedAt = DateTime.Now
                }));
            }

            loadMoreEnabled = true;

            IsBusy = false;
        }

        public override async Task AddItem(UserAnamnesisViewModel item)
        {
            IsBusy = true;

            item.UserId = userService.UserId;

            var userAnamnesis = await anamnesisService.SavePharmacyAnamnesis(item);
            if (userAnamnesis != null)
            {
                MessagingCenter.Instance.Send(this, "OnPharmacyAnamnesisCreated", userAnamnesis);
                await popupNavigation.PopAsync();
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

            request.Filter = SearchQuery;
            request.SecondFilter = SecondSearchQuery;
            request.Page = DefaulPage;

            var pharmacies = await pharmacyService.GetPharmacyTable(request);
            if (pharmacies != null)
            {
                itemsTotalCountDb = pharmacies.TotalCount;
                Items = new ObservableCollection<UserAnamnesisViewModel>(pharmacies.Data.Select(a => new UserAnamnesisViewModel(a)));
            }


            IsBusy = false;
        }
    }
}
