using MyHeart.MobileApp.ViewModels.MyAnamnesis.Physical;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MyHeart.MobileApp.Views;
using MyHeart.MobileApp.Services;
using System.Collections.ObjectModel;
using System.Linq;
using MyHeart.MobileApp.Views.MyAnamnesis;

namespace MyHeart.MobileApp.ViewModels
{
    public class PhysicalCardViewModel : AnamesisCardViewModel<UserAnamnesisDTO>
    {
        AnamnesisService anamnesisService;
        private readonly List<UserAnamnesisDTO> records = new List<UserAnamnesisDTO>();
        public bool HasTypeCholesterol { get => true; }
        public PhysicalCardViewModel()
        {
            anamnesisService = DependencyService.Resolve<AnamnesisService>();

            //MessagingCenter.Instance.Subscribe<NewPersonalAnamnessesPageViewModel>(this, "OnUserDiseaseCreated", sender => LoadData());

            _ = LoadData();
        }
        public override async Task GoToAddItem()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new PhysicalPage
            {
                BindingContext = new PhysicalListViewModel(Items)
            });
        }

        public override Task DeleteItem(UserAnamnesisDTO item)
        {
            throw new NotImplementedException();
        }

        public override async Task LoadData()
        {
            IsBusy = true;

            var diseaseAnamneses = await anamnesisService.GetPersonalAsync();

            if (diseaseAnamneses != null)
            {
                Items = new ObservableCollection<UserAnamnesisDTO>(diseaseAnamneses);
                foreach (var item in diseaseAnamneses)
                {
                    Rows.Add(new AnamnesisCardRow
                    {
                        Id = item.Id,
                        Title = item.IsPersonal_Type.ToString(),
                        Value = item.IsPersonal_Value
                    });
                }

            }

            IsBusy = false;
        }

        public override async Task GoToDetail(int id)
        {
            throw new NotImplementedException();
        }
    }
}
