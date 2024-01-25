using MyHeart.DTO;
using MyHeart.DTO.User;
using MyHeart.MobileApp.ViewModels.MyAnamnesis;
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
    public class MyIllnessListViewModel : MyAnamnesisListViewModel<UserDiseaseViewModel>
    {
        public MyIllnessListViewModel(ObservableCollection<UserDiseaseViewModel> items)
        {
            MessagingCenter.Instance.Subscribe<AddMyIllnessPageViewModel, UserDiseaseDto>(this, "DiseaseAnamnesisCreated", (sender, item) => ApplyAddedChanges(item));
            MessagingCenter.Instance.Subscribe<MyIllnessDetailPageViewModel, UserDiseaseDto>(this, "DiseaseAnamnesisUpdated", (sender, item) => ApplyUpdatedChanges(item));
            MessagingCenter.Instance.Subscribe<MyIllnessDetailPageViewModel, int>(this, "DiseaseAnamnesisDeleted", (sender, id) => ApplyDeletedChanges(id));

            Items = new ObservableCollection<UserDiseaseViewModel>(items);
        }

        protected override async Task GoToCreateNew()
        {
            await popupNavigation.PushAsync(new AnamnesisAddPage("Moje onemocnění")
            {
                ItemsView = new MyIllnessCollectionView(),
                BindingContext = new AddMyIllnessPageViewModel()
            });
        }

        public void ApplyDeletedChanges(int id)
        {
            var disease = Items.FirstOrDefault(i => i.Id == id);
            if (disease != null)
            {
                Items.Remove(disease);
            }
        }

        public void ApplyAddedChanges(UserDiseaseDto item)
        {
            Items.Add(new UserDiseaseViewModel(item));
        }

        public void ApplyUpdatedChanges(UserDiseaseDto item)
        {
            var disease = Items.FirstOrDefault(i => i.Id == item.Id);
            if (disease != null)
            {
                var index = Items.IndexOf(disease);
                Items[index] = new UserDiseaseViewModel(item);
            }
        }

        protected override async Task GoToDetail(UserDiseaseViewModel item)
        {
            var itemCopy = new UserDiseaseViewModel
            {
                Disease = item.Disease,
                DiseaseId = item.DiseaseId,
                Id = item.Id,
                Note = item.Note,
                StartDate = item.StartDate,
                StartDateString = item.StartDateString,
                User = item.User,
                UserId = item.UserId,
            };

            await popupNavigation.PushAsync(new AnamnesisDetailPage
            {
                ItemView = new MyIllnessDetailView(),
                BindingContext = new MyIllnessDetailPageViewModel(itemCopy),
                Title = "Detail výkonu"
            });
        }
    }
}
