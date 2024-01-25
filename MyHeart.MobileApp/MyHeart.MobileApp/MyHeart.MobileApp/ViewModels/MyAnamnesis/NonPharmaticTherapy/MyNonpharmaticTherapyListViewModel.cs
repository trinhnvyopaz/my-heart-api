using MyHeart.DTO;
using MyHeart.MobileApp.Models;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.ViewModels.MyAnamnesis;
using MyHeart.MobileApp.ViewModels.MyAnamnesis.MyAlergy;
using MyHeart.MobileApp.ViewModels.MyAnamnesis.NonPharmaticTherapy;
using MyHeart.MobileApp.Views.Controls;
using MyHeart.MobileApp.Views.MyAnamnesis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public class MyNonpharmaticTherapyListViewModel : MyAnamnesisListViewModel<UserNonpharmacyViewModel>
    {
        public MyNonpharmaticTherapyListViewModel(ObservableCollection<UserNonpharmacyViewModel> nonpharmacies)
        {
            MessagingCenter.Instance.Subscribe<NonpharmaticTherapyDetailViewModel, UserNonpharmacyDto>(this, "UserNonPHarmacyUpdated", (sender, item) => ApplyUpdateChanges(item));
            MessagingCenter.Instance.Subscribe<AddUserNonpharmacyPageViewModel, UserNonpharmacyDto>(this, "OnUserNonpharmacyCreated", (sender, item) => ApplyAddChanges(item));
            MessagingCenter.Instance.Subscribe<NonpharmaticTherapyDetailViewModel, int>(this, "UserNonPHarmacyDeleted", (sender, id) => ApplyDeleteChanges(id));

            Items = new ObservableCollection<UserNonpharmacyViewModel>(nonpharmacies);
        }

        protected override async Task GoToCreateNew()
        {
            await popupNavigation.PushAsync(new AnamnesisAddPage("Provedené výkony")
            {
                ItemsView = new NonPharmacyCollectionView(),
                BindingContext = new AddUserNonpharmacyPageViewModel()
            });
        }

        private void ApplyDeleteChanges(int id)
        {
            var nonpharmacy = Items.FirstOrDefault(i => i.Id == id);
            if (nonpharmacy != null)
            {
                Items.Remove(nonpharmacy);
            }
        }

        private void ApplyAddChanges(UserNonpharmacyDto item)
        {
            Items.Add(new UserNonpharmacyViewModel(item));
        }

        private void ApplyUpdateChanges(UserNonpharmacyDto item)
        {
            var nonpharmacy = Items.FirstOrDefault(i => i.Id == item.Id);
            if (nonpharmacy != null)
            {
                var index = Items.IndexOf(nonpharmacy);
                Items[index] = new UserNonpharmacyViewModel(item);
            }
        }

        protected override async Task GoToDetail(UserNonpharmacyViewModel item)
        {
            var itemCopy = (UserNonpharmacyViewModel)item.Clone();

            await popupNavigation.PushAsync(new AnamnesisDetailPage
            {
                ItemView = new NonPharmacyDetailView(),
                BindingContext = new NonpharmaticTherapyDetailViewModel(itemCopy),
                Title = "Detail výkonu"
            });
        }
    }
}
