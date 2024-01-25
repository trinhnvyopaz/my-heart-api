using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.ViewModels.MyAnamnesis.MyAlergy;
using MyHeart.MobileApp.Views.MyAnamnesis.MyAllergy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public class MyAlergyCardViewModel : AnamesisCardViewModel<UserAnamnesisDTO>
    {
        private AnamnesisService anamnesisService;

        public MyAlergyCardViewModel()
        {
            anamnesisService = DependencyService.Resolve<AnamnesisService>();

            MessagingCenter.Instance.Subscribe<CreateAllergyPopUpViewModel, UserAnamnesisDTO>(this, "AlergyAnamnesisCreated", (sender, item) => LoadData());
            MessagingCenter.Instance.Subscribe<AlergyDetailPageViewModel, UserAnamnesisDTO>(this, "AlergyAnamnesisUpdated", (sender, item) => LoadData());
            MessagingCenter.Instance.Subscribe<AlergyDetailPageViewModel, int>(this, "AlergyAnamnesisDeleted", (sender, id) => LoadData());

            _ = LoadData();
        }
        public override async Task DeleteItem(UserAnamnesisDTO item)
        {
            bool result = await anamnesisService.DeleteAllergyAsync(item.Id);

            if(result)
            {
                NotifyDeleted();
                Items.Remove(item);
            }
            else
            {
                NotifyDeleteFailed();
            }
        }

        public override async Task GoToAddItem()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new MyAllergyListPage(Items));
        }

        public override async Task LoadData()
        {
            IsBusy = true;

            var allergies = await anamnesisService.GetAllergyAsync();

            if(allergies != null)
            {
                Items = new ObservableCollection<UserAnamnesisDTO>(allergies);
                Rows = new ObservableCollection<AnamnesisCardRow>();

                foreach (var item in allergies)
                {
                    Rows.Add(new AnamnesisCardRow
                    {
                        Id = item.Id,
                        Title = item.IsAllergy_Name
                    });
                }
            }

            IsBusy = false;
        }
    }
}
