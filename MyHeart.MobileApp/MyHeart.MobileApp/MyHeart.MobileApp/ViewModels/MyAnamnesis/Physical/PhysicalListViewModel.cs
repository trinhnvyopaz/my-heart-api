using MyHeart.DTO.User;
using MyHeart.MobileApp.ViewModels.MyAnamnesis;
using MyHeart.MobileApp.Views.Controls;
using MyHeart.MobileApp.Views.MyAnamnesis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.MobileApp.ViewModels
{
    internal class PhysicalListViewModel : MyAnamnesisListViewModel<UserAnamnesisDTO>
    {

        public PhysicalListViewModel(ObservableCollection<UserAnamnesisDTO> items)
        {
            Items = items;
        }

        protected override async Task GoToCreateNew()
        {
            await popupNavigation.PushAsync(new AnamnesisAddPage("")
            {
                ItemsView = new MyTreatmentCollectionView(),
                BindingContext = new AddUserNonpharmacyPageViewModel()
            });
        }

        protected override async Task GoToDetail(UserAnamnesisDTO item)
        {
            //await popupNavigation.PushAsync(new AnamnesisDetailPage
            //{
            //    ItemView = new MyTreatmentDetailView(),
            //    BindingContext = new MyTreatmentDetailPageViewModel(item),
            //    Title = "Detail léčby",
            //});
        }
    }
}
