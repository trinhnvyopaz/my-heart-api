using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels.MyAnamnesis.NonPharmaticTherapy
{
    public class NonpharmaticTherapyDetailViewModel : AnamnesisDetailPageViewModel<UserNonpharmacyViewModel>
    {
        private UserNonPharmacyService userNonpharmacyService;

        public NonpharmaticTherapyDetailViewModel(UserNonpharmacyViewModel item) : base(item)
        {
            userNonpharmacyService = DependencyService.Resolve<UserNonPharmacyService>();
        }
        public override async Task SaveItem()
        {
            var userNonPharmacy = await userNonpharmacyService.SaveUserNonpharmacy(Item);
            if (userNonPharmacy != null)
            {
                MessagingCenter.Instance.Send(this, "UserNonPHarmacyUpdated", userNonPharmacy);
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
            var result = await userNonpharmacyService.DeleteNonPharmaticTherapies(Item.Id);
            if (result)
            {
                await GoBack();
                MessagingCenter.Instance.Send(this, "UserNonPHarmacyDeleted", Item.Id);
                NotifyDeleted();
            }
            else
            {
                NotifyDeleteFailed();
            }
        }
    }
}
