using MyHeart.DTO;
using MyHeart.DTO.User;
using MyHeart.MobileApp.ViewModels.MyAnamnesis;
using MyHeart.MobileApp.Views.Controls;
using MyHeart.MobileApp.Views.MyAnamnesis;
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
    public class MyTreatmentListViewModel : MyAnamnesisListViewModel<UserAnamnesisViewModel>
    {
        public MyTreatmentListViewModel(ObservableCollection<UserAnamnesisViewModel> items)
        {
            Items = new ObservableCollection<UserAnamnesisViewModel>(items);

            MessagingCenter.Instance.Subscribe<AddMyTreatmentPageViewModel, UserAnamnesisDTO>(this, "OnPharmacyAnamnesisCreated", (sender, item) => ApplyAddChanges(item));
            MessagingCenter.Instance.Subscribe<MyTreatmentDetailPageViewModel, UserAnamnesisDTO>(this, "PharmacyAnamnesisUpdated", (sender, item) => ApplyUpdateChanges(item));
            MessagingCenter.Instance.Subscribe<MyTreatmentDetailPageViewModel, int>(this, "PharmacyAnamnesisDeleted", (sender, id) => ApplyDeleteChanges(id));
        }

        private void ApplyDeleteChanges(int id)
        {
            var nonpharmacy = Items.FirstOrDefault(i => i.Id == id);
            if (nonpharmacy != null)
            {
                Items.Remove(nonpharmacy);
            }
        }

        private void ApplyAddChanges(UserAnamnesisDTO item)
        {
            Items.Add(new UserAnamnesisViewModel(item));
        }

        private void ApplyUpdateChanges(UserAnamnesisDTO item)
        {
            var nonpharmacy = Items.FirstOrDefault(i => i.Id == item.Id);
            if (nonpharmacy != null)
            {
                var index = Items.IndexOf(nonpharmacy);
                Items[index] = new UserAnamnesisViewModel(item);
            }
        }

        protected override async Task GoToCreateNew()
        {

            await popupNavigation.PushAsync(new AnamnesisAddPage("Moje léčba")
            {
                ItemsView = new MyTreatmentCollectionView(),
                BindingContext = new AddMyTreatmentPageViewModel()
            });
        }

        protected override async Task GoToDetail(UserAnamnesisViewModel item)
        {
            var itemCopy = (UserAnamnesisViewModel)item.Clone();

            await popupNavigation.PushAsync(new AnamnesisDetailPage
            {
                ItemView = new MyTreatmentDetailView(),
                BindingContext = new MyTreatmentDetailPageViewModel(itemCopy),
                Title = "Detail léčby",
            });
        }
    }
}
