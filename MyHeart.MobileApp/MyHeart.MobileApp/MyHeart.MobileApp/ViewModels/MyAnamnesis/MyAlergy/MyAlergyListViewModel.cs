using MyHeart.DTO.User;
using MyHeart.MobileApp.Models;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.ViewModels.MyAnamnesis;
using MyHeart.MobileApp.ViewModels.MyAnamnesis.MyAlergy;
using MyHeart.MobileApp.ViewModels.MyAnamnesis.NonPharmaticTherapy;
using MyHeart.MobileApp.Views.Controls;
using MyHeart.MobileApp.Views.MyAnamnesis;
using MyHeart.MobileApp.Views.MyAnamnesis.MyAllergy;
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
    public class MyAlergyListViewModel : MyAnamnesisListViewModel<UserAnamnesisDTO>
    {
        public MyAlergyListViewModel(ObservableCollection<UserAnamnesisDTO> allergies)
        {
            MessagingCenter.Instance.Subscribe<CreateAllergyPopUpViewModel, UserAnamnesisDTO>(this, "AlergyAnamnesisCreated", (sender, item) => ApplyAddedChanges(item));
            MessagingCenter.Instance.Subscribe<AlergyDetailPageViewModel, UserAnamnesisDTO>(this, "AlergyAnamnesisUpdated", (sender, item) => ApplyUpdatedChanges(item));
            MessagingCenter.Instance.Subscribe<AlergyDetailPageViewModel, int>(this, "AlergyAnamnesisDeleted", (sender, id) => ApplyDeletedChanges(id));

            Items = new ObservableCollection<UserAnamnesisDTO>(allergies);
        }

        private void ApplyAddedChanges(UserAnamnesisDTO item)
        {
            Items.Add(item);
        }


        private void ApplyUpdatedChanges(UserAnamnesisDTO item)
        {
            var allergy = Items.FirstOrDefault(i => i.Id == item.Id);
            if (allergy != null)
            {
                allergy = item;
            }
        }

        private void ApplyDeletedChanges(int id)
        {
            var item = Items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        protected override async Task GoToDetail(UserAnamnesisDTO item)
        {
            await popupNavigation.PushAsync(new AnamnesisDetailPage
            {
                ItemView = new AlergyDetailView(),
                BindingContext = new AlergyDetailPageViewModel(new UserAnamnesisViewModel(item)),
                Subtitle = "Alergie",
                HeaderFontColor = (Color)App.Current.Resources["DarkFontColor"]
            });
        }

        protected override async Task GoToCreateNew()
        {
            await popupNavigation.PushAsync(new CreateAllergyPopUp
            {
                BindingContext = new CreateAllergyPopUpViewModel()
            });
        }
    }
}
