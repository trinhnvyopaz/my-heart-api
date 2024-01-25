using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels.PopUps
{
    public class SmokerPopUpViewModel : BasePopUpViewModel
    {
        private AnamnesisService anamnesisService;
        private UserAnamnesisViewModel abususAnamnesis;

        public UserAnamnesisViewModel AbususAnamnesis
        {
            get => abususAnamnesis;
            set => SetProperty(ref abususAnamnesis, value);
        }
        public SmokerPopUpViewModel(UserAnamnesisDTO userAnamnesis)
        {
            if(userAnamnesis == null)
            {
                AbususAnamnesis = new UserAnamnesisViewModel();
            }
            else
            {
                AbususAnamnesis = new UserAnamnesisViewModel(userAnamnesis);
            }

           
            anamnesisService = DependencyService.Resolve<AnamnesisService>();
        }
        protected override async Task SaveItem()
        {
            IsBusy = true;

            var userAnamnesis = await anamnesisService.SaveAbususAsync(AbususAnamnesis);
            if (userAnamnesis != null)
            {
                
                MessagingCenter.Instance.Send(this as BasePopUpViewModel, "UserUpdated");
                GoBackCommand.Execute(null);
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
