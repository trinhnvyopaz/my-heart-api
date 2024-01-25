using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public class MyTreatmentDetailPageViewModel : AnamnesisDetailPageViewModel<UserAnamnesisViewModel>
    {
        private AnamnesisService anamnesisService;

        public MyTreatmentDetailPageViewModel(UserAnamnesisViewModel userAnamnesis) : base(userAnamnesis)
        {
            anamnesisService = DependencyService.Resolve<AnamnesisService>();
        }

        public override async Task SaveItem()
        {
            var pharmacyAnamnesis = await anamnesisService.SavePharmacyAnamnesis(Item);
            if (pharmacyAnamnesis != null)
            {
                MessagingCenter.Instance.Send(this, "PharmacyAnamnesisUpdated", pharmacyAnamnesis);
                await popupNavigation.PopAsync();
                NotifyUpdated();
            }
            else
            {
                NotifyUpdateFailed();
            }
        }

        public override async Task GoBack()
        {
            await popupNavigation.PopAsync();
        }

        public override async Task DeleteItem()
        {
            bool result = await anamnesisService.DeletePharmacyAnamnesis(Item.Id);
            if (result)
            {
                await popupNavigation.PopAsync();
                MessagingCenter.Instance.Send(this, "PharmacyAnamnesisDeleted", Item.Id);
                NotifyDeleted();
            }
            else
            {
                NotifyDeleteFailed();
            }
        }
    }
}
