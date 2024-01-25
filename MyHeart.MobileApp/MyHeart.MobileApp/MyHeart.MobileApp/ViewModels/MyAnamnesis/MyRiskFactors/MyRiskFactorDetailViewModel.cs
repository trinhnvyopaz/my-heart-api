using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public class MyRiskFactorDetailViewModel : BaseViewModel
    {
        private AnamnesisService anamnesisService;
        private UserAnamnesisViewModel familyAnamnesis;
        private UserAnamnesisViewModel abususAnamnesis;
        private UserAnamnesisViewModel socialAnamnesis;

        public UserAnamnesisViewModel FamilyAnamnesis
        {
            get => familyAnamnesis;
            set => SetProperty(ref familyAnamnesis, value);
        }

        public UserAnamnesisViewModel AbususAnamnesis
        {
            get => abususAnamnesis;
            set => SetProperty(ref abususAnamnesis, value);
        }
        public UserAnamnesisViewModel SocialAnamnesis
        {
            get => socialAnamnesis;
            set => SetProperty(ref socialAnamnesis, value);
        }

        public ICommand SaveCommand { get; set; }
        public ICommand GoBackCommand { get; set; }

        public MyRiskFactorDetailViewModel()
        {
            anamnesisService = DependencyService.Resolve<AnamnesisService>();

            SaveCommand = new Command(() => SaveItem());
            GoBackCommand = new Command(() => Application.Current.MainPage.Navigation.PopModalAsync());

            _ = LoadData();
        }

        public async Task LoadData()
        {
            IsBusy = true;

            var family = await anamnesisService.GetFamilyAsync();
            var social = await anamnesisService.GetSocialAsync();
            var abusus = await anamnesisService.GetAbususAsync();

            if (family == null)
            {
                FamilyAnamnesis = new UserAnamnesisViewModel();
            }
            else
            {
                FamilyAnamnesis = new UserAnamnesisViewModel(family);
            }
            if (abusus == null)
            {
                AbususAnamnesis = new UserAnamnesisViewModel();
            }
            else
            {
                AbususAnamnesis = new UserAnamnesisViewModel(abusus);
            }
            if (social == null)
            {
                SocialAnamnesis = new UserAnamnesisViewModel();
            }
            else
            {
                SocialAnamnesis = new UserAnamnesisViewModel(social);
            }

            IsBusy = false;
        }


        public async Task SaveItem()
        {
            IsBusy = true;

            var familyAnamnesisApiTask = anamnesisService.SaveFamilyAnamnesis(FamilyAnamnesis);
            var abususAnamnesisApiTask = anamnesisService.SaveAbususAsync(AbususAnamnesis);
            var socialAnamnesisApiTask = anamnesisService.SaveSocialAnamnesis(SocialAnamnesis);

            await Task.WhenAll(familyAnamnesisApiTask, abususAnamnesisApiTask, socialAnamnesisApiTask);

            if (familyAnamnesisApiTask.Result != null && abususAnamnesisApiTask.Result != null && socialAnamnesisApiTask.Result != null)
            {
                MessagingCenter.Instance.Send(this, "FamilyAnamnesisUpdated");
                await Application.Current.MainPage.Navigation.PopAsync();
                NotifyUpdated();
            }
            else
            {
                NotifyUpdateFailed();
            }

            IsBusy = false;
        }
    }
}
