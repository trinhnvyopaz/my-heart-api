using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.ViewModels;
using MyHeart.MobileApp.Views.Controls;
using MyHeart.MobileApp.Views.MyAnamnesis;
using MyHeart.MobileApp.Views.MyAnamnesis.MyIllness;
using MyHeart.MobileApp.Views.MyAnamnesis.NonPharmaticTherapy;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
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
    public class MyIllnessCardViewModel : AnamesisCardViewModel<UserDiseaseViewModel>
    {
        AnamnesisService anamnesisService;

        public MyIllnessCardViewModel()
        {
            anamnesisService = DependencyService.Resolve<AnamnesisService>();

            MessagingCenter.Instance.Subscribe<AddMyIllnessPageViewModel, UserDiseaseDto>(this, "DiseaseAnamnesisCreated", (sender, item) => LoadData());
            MessagingCenter.Instance.Subscribe<MyIllnessDetailPageViewModel, UserDiseaseDto>(this, "DiseaseAnamnesisUpdated", (sender, item) => LoadData());
            MessagingCenter.Instance.Subscribe<MyIllnessDetailPageViewModel, int>(this, "DiseaseAnamnesisDeleted", (sender, id) => LoadData());

            _ = LoadData();
        }

        public override async Task GoToAddItem()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new MyIllnessListPage(Items));
        }

        public override async Task GoToDetail(int id)
        {
            var item = Items.First(i => i.Id == id);

            await popupNavigation.PushAsync(new AnamnesisDetailPage
            {
                ItemView = new MyIllnessDetailView(),
                BindingContext = new MyIllnessDetailPageViewModel(item),
                Title = "Detail onemocnění"
            });
        }

        public override async Task LoadData()
        {
            IsBusy = true;

            var diseaseAnamneses = await anamnesisService.GetDiseaseAnamneses();

            if (diseaseAnamneses != null)
            {
                Items = new ObservableCollection<UserDiseaseViewModel>();
                Rows = new ObservableCollection<AnamnesisCardRow>();

                foreach (var item in diseaseAnamneses)
                {
                    Items.Add(new UserDiseaseViewModel(item));
                    Rows.Add(new AnamnesisCardRow
                    {
                        Id = item.Id,
                        Title = item.Disease.Name
                    });
                }
            }

            IsBusy = false;
        }
    }
}
