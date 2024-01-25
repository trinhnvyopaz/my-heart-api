using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels.PopUps
{
    public class PhysicalActivityPopUpViewModel : BasePopUpViewModel
    {
        private AnamnesisService anamnesisService;
        private UserAnamnesisViewModel userAnamnesis;
        private PhysicalActivityFrequencyType selectedValue;

        public UserAnamnesisViewModel UserAnamnesis
        {
            get => userAnamnesis;
            set => SetProperty(ref userAnamnesis, value);
        }

        public PhysicalActivityFrequencyType SelectedValue
        {
            get => selectedValue;
            set
            {
                SetProperty(ref selectedValue, value);
                userAnamnesis.IsGeneral_PhysicalActivityFrequencyType= value;
            }
        }

        public ICommand ChangeSelectionCommand { get; set; }

        public PhysicalActivityPopUpViewModel(UserAnamnesisDTO userAnamnesis)
        {
            anamnesisService = DependencyService.Resolve<AnamnesisService>();

            if (userAnamnesis == null)
            {
                UserAnamnesis = new UserAnamnesisViewModel();
            }
            else
            {
                UserAnamnesis = new UserAnamnesisViewModel(userAnamnesis);
            }

            selectedValue = UserAnamnesis.IsGeneral_PhysicalActivityFrequencyType;

            ChangeSelectionCommand = new Command<PhysicalActivityFrequencyType>(ChangeSelection);
        }

        private void ChangeSelection(PhysicalActivityFrequencyType value)
        {
            SelectedValue = value;
        }

        protected override async Task SaveItem()
        {
            var userAnamnesis = await anamnesisService.SaveGeneralAsync(UserAnamnesis);
            if (userAnamnesis != null)
            {
                MessagingCenter.Instance.Send(this as BasePopUpViewModel, "UserAnamnesisUpdated");
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
