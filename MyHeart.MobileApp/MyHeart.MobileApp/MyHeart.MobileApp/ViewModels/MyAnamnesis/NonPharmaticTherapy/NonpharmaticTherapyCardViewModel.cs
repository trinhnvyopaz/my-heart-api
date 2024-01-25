using MyHeart.DTO;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.ViewModels.MyAnamnesis.NonPharmaticTherapy;
using MyHeart.MobileApp.Views;
using MyHeart.MobileApp.Views.Controls;
using MyHeart.MobileApp.Views.MyAnamnesis;
using MyHeart.MobileApp.Views.MyAnamnesis.NonPharmaticTherapy;
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
    public class NonpharmaticTherapyCardViewModel : AnamesisCardViewModel<UserNonpharmacyViewModel>
    {
        private UserNonPharmacyService _nonPharmacyService;

        public NonpharmaticTherapyCardViewModel()
        {
            _nonPharmacyService = DependencyService.Resolve<UserNonPharmacyService>(); ;

            MessagingCenter.Instance.Subscribe<AddUserNonpharmacyPageViewModel, UserNonpharmacyDto>(this, "OnUserNonpharmacyCreated", (sender, item) => LoadData());
            MessagingCenter.Instance.Subscribe<NonpharmaticTherapyDetailViewModel, UserNonpharmacyDto>(this, "UserNonPHarmacyUpdated", (sender, item) => LoadData());
            MessagingCenter.Instance.Subscribe<NonpharmaticTherapyDetailViewModel, int>(this, "UserNonPHarmacyDeleted", (sender, id) => LoadData());

            _ = LoadData();
        }

        public override async Task LoadData()
        {
            IsBusy = true;

            var userNonphamaciesList = await _nonPharmacyService.GetUserNonpharmacies();

            if (userNonphamaciesList != null)
            {
                Items = new ObservableCollection<UserNonpharmacyViewModel>();
                Rows = new ObservableCollection<AnamnesisCardRow>();

                foreach (var item in userNonphamaciesList)
                {
                    Items.Add(new UserNonpharmacyViewModel(item));
                    Rows.Add(new AnamnesisCardRow
                    {
                        Id = item.Id,
                        Title = item.NonpharmaticTherapy.Name,
                        Value = item.LastUpdateDate.ToString("dd.MM.yyy")
                    });
                }
            }

            IsBusy = false;
        }

        public override async Task GoToAddItem()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new MyNonpharmaticTherapyListPage(Items));
        }

        public override async Task GoToDetail(int id)
        {
            var item = Items.First(i => i.Id == id);
            await popupNavigation.PushAsync(new AnamnesisDetailPage
            {
                ItemView = new NonPharmacyDetailView(),
                BindingContext = new NonpharmaticTherapyDetailViewModel(item),
                Title = "Detail výkonu"
            });
        }
    }
}
