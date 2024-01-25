using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels.MyAnamnesis.MyAlergy
{
    public class CreateAllergyPopUpViewModel : BasePopUpViewModel
    {
        private AnamnesisService anamnesisService;
        private UserService userService;
        private string allergyName;

        public string AllergyName
        {
            get => allergyName;
            set => SetProperty(ref allergyName, value);
        }
        public CreateAllergyPopUpViewModel()
        {
            anamnesisService = DependencyService.Resolve<AnamnesisService>();           
            userService = DependencyService.Resolve<UserService>();
        }

        protected override async Task SaveItem()
        {
            var vm = new UserAnamnesisViewModel
            {
                IsPersonal_Type =0,
                IsPersonal_Date = new DateTime(),
                IsPersonal_Value = "",
                IsAllergy_Name= allergyName,
                UserId = userService.UserId
            };

            var allergy = await anamnesisService.SaveAllergyAnamnesis(vm);
            if(allergy != null)
            {
                
                MessagingCenter.Instance.Send(this, "AlergyAnamnesisCreated", allergy);
                toastService.ShowSuccess("Podařilo se vytvořit alegii");
                GoBackCommand.Execute(null);                
            }
            else
            {
                toastService.ShowError("Nepodařilo se vytvořit alegii");
            }
        }
    }
}
