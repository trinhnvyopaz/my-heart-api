using DevExpress.XamarinForms.Charts;
using MyHeart.DTO.User;
using MyHeart.MobileApp.Enums;
using MyHeart.MobileApp.Services;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing;

namespace MyHeart.MobileApp.ViewModels.PopUps
{
    public class PersonalAnamnesisPopUpViewModel : BasePopUpViewModel
    {
        private bool isNew;

        private PersonalRecordViewModel personalAnamnesis;
        private AnamnesisService anamnesisService;

        IPopupNavigation popupNavigation = PopupNavigation.Instance;

        public bool IsNew
        {
            get => isNew;
            set => SetProperty(ref isNew, value);
        }

        public PersonalRecordViewModel PersonalAnamnesis
        {
            get => personalAnamnesis;
            set => SetProperty(ref personalAnamnesis, value);
        }


        public ICommand DeleteItemCommand { get; set; }

        public PersonalAnamnesisPopUpViewModel(UserAnamnesisDTO dto, AnamnesisType anamnesisType)
        {
            if (dto == null)
            {
                IsNew = true;
                var now = DateTime.Now;
                PersonalAnamnesis = new PersonalRecordViewModel
                {                   
                    Type = anamnesisType,
                    Date= now.Date,
                    Time = now.TimeOfDay
                };
            }
            else
            {
                PersonalAnamnesis = new PersonalRecordViewModel(dto);
            }

            anamnesisService = DependencyService.Resolve<AnamnesisService>();

            DeleteItemCommand = new Command(() => _ = DeleteItem());
        }


        private async Task DeleteItem()
        {
            var result = await anamnesisService.DeletePersonalAnamnesis(PersonalAnamnesis.Id);
            if (result)
            {
                toastService.ShowSuccess("Záznam úspěšně smazán");
                MessagingCenter.Send(this, "PersonalAnamnesisDelete", PersonalAnamnesis.Id);
                MessagingCenter.Send(this as BasePopUpViewModel, "UserAnamnesisUpdated");
                GoBack();
            }
            else
            {
                toastService.ShowError("Záznam se nepodařilo smazat");
            }
        }

        private async Task CreateItem()
        {
            var personalAnamnesis = await anamnesisService.SavePersonalAsync(PersonalAnamnesis);
            if (personalAnamnesis != null)
            {
                toastService.ShowSuccess("Záznam úspěšně vytvořen");
                MessagingCenter.Send(this, "PersonalAnamnesisCreate", personalAnamnesis);
                MessagingCenter.Send(this as BasePopUpViewModel, "UserAnamnesisUpdated");
                GoBack();
            }
            else
            {
                toastService.ShowError("Záznam se nepodařilo vytvořit");
            }
        }

        private async Task UpdateItem()
        {
            var personalAnamnesis = await anamnesisService.UpdatePersonalAsync(PersonalAnamnesis);
            if (personalAnamnesis != null)
            {
                toastService.ShowSuccess("Záznam úspěšně uložen");
                MessagingCenter.Send(this, "PersonalAnamnesisUpdate", personalAnamnesis);
                MessagingCenter.Send(this as BasePopUpViewModel, "UserAnamnesisUpdated");
                GoBack();
            }
            else
            {
                toastService.ShowError("Záznam se nepodařilo uložit");
            }
        }

        private async void GoBack()
        {
            await popupNavigation.PopAsync();
        }

        protected override async Task SaveItem()
        {
            if (isNew)
            {
                await CreateItem();
            }
            else
            {
                await UpdateItem();
            }
        }
    }
}
