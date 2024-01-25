using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public class MyIllnessDetailPageViewModel : AnamnesisDetailPageViewModel<UserDiseaseViewModel>
    {
        private AnamnesisService anamnesisService;

        public MyIllnessDetailPageViewModel(UserDiseaseViewModel userDisease) : base(userDisease)
        {
            anamnesisService = DependencyService.Resolve<AnamnesisService>();
        }
        public override async Task SaveItem()
        {
            var diseaseAnamnesis = await anamnesisService.SaveDiseaseAnamnesis(Item);

            if (diseaseAnamnesis != null)
            {
                MessagingCenter.Instance.Send(this, "DiseaseAnamnesisUpdated", diseaseAnamnesis);
                await GoBack();
                NotifyUpdated();
            }
            else
            {
                NotifyUpdateFailed();
            }
        }

        public override async Task DeleteItem()
        {
            var result = await anamnesisService.DeleteDiseaseAnamnesis(Item.Id);
            if (result)
            {
                await GoBack();
                MessagingCenter.Instance.Send(this, "DiseaseAnamnesisDeleted", Item.Id);
                NotifyDeleted();
            }
            else
            {
                NotifyDeleteFailed();
            }
        }
    }
}
