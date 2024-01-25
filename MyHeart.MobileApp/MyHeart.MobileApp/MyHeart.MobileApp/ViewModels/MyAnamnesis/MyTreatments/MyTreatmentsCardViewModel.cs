using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.ViewModels;
using MyHeart.MobileApp.Views;
using MyHeart.MobileApp.Views.Controls;
using MyHeart.MobileApp.Views.MyAnamnesis;
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
    public class MyTreatmentsCardViewModel : AnamesisCardViewModel<UserAnamnesisViewModel>
    {
        private AnamnesisService anamnesisService;
        public MyTreatmentsCardViewModel()
        {
            anamnesisService = DependencyService.Resolve<AnamnesisService>();

            MessagingCenter.Instance.Subscribe<AddMyTreatmentPageViewModel, UserAnamnesisDTO>(this, "OnPharmacyAnamnesisCreated", (sender, item) => LoadData());
            MessagingCenter.Instance.Subscribe<MyTreatmentDetailPageViewModel, UserAnamnesisDTO>(this, "PharmacyAnamnesisUpdated", (sender, Item) => LoadData());
            MessagingCenter.Instance.Subscribe<MyTreatmentDetailPageViewModel, int>(this, "PharmacyAnamnesisDeleted", (sender, id) => LoadData());

            _ = LoadData();
        }

        public override async Task GoToAddItem()
        {        
            await Application.Current.MainPage.Navigation.PushModalAsync(new MyTreatmentListPage
            {
                BindingContext = new MyTreatmentListViewModel(Items)
            });
        }

        public override async Task LoadData()
        {
            IsBusy = true;

            var userAnamnesesList = await anamnesisService.GetPharmacyAnamnesis();
            if (userAnamnesesList != null)
            {
                Items = new ObservableCollection<UserAnamnesisViewModel>();
                Rows = new ObservableCollection<AnamnesisCardRow>();

                foreach (var item in userAnamnesesList)
                {
                    var itemVM = new UserAnamnesisViewModel(item);

                    bool isLast = userAnamnesesList.LastOrDefault() == item;
                    itemVM.IsLastRow = isLast;

                    Items.Add(itemVM);

                    var row = new AnamnesisCardRow
                    {
                        Id = itemVM.Id,
                        Title = itemVM.IsPharmacy_Name,
                        Value = $"{itemVM.IsPharmacy_MorningDose}-{itemVM.IsPharmacy_AfternoonDose}-{itemVM.IsPharmacy_EveningDose}"
                    };

                    Rows.Add(row);
                }
            }

            IsBusy = false;
        }
    }
}
