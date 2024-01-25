using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels.MyAnamnesis.MyAlergy
{
    public class AlergyDetailPageViewModel : AnamnesisDetailPageViewModel<UserAnamnesisViewModel>
    {
        private AnamnesisService anamnesisService;
        
        public AlergyDetailPageViewModel(UserAnamnesisViewModel item) : base(item)
        {
            anamnesisService = DependencyService.Resolve<AnamnesisService>();
        }

        public override async Task SaveItem()
        {
            var pharmacyAnamnesis = await anamnesisService.SaveAllergyAnamnesis(Item);
            if (pharmacyAnamnesis != null)
            {
                MessagingCenter.Instance.Send(this, "AlergyAnamnesisUpdated", pharmacyAnamnesis);
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
            bool result = await anamnesisService.DeleteAllergyAsync(Item.Id);
            if (result)
            {
                await popupNavigation.PopAsync();
                MessagingCenter.Instance.Send(this, "AlergyAnamnesisDeleted", Item.Id);
                NotifyDeleted();
            }
            else
            {
                NotifyDeleteFailed();
            }
        }
    }
}
