using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.Views.MyAnamnesis.MyRiskFactors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
namespace MyHeart.MobileApp.ViewModels

{
    public class MyRiskFactorsCardViewModel : BaseViewModel
    {
        private AnamnesisService anamnesisService;
        private UserAnamnesisDTO familyItem;
        private UserAnamnesisDTO socialItem;
        private UserAnamnesisDTO abususItem;

        public UserAnamnesisDTO FamilyItem
        {
            get => familyItem;
            set => SetProperty(ref familyItem, value);
        }

        public UserAnamnesisDTO SocialItem
        {
            get => socialItem;
            set => SetProperty(ref socialItem, value);
        }

        public UserAnamnesisDTO AbususItem
        {
            get => abususItem;
            set => SetProperty(ref abususItem, value);
        }

        public ICommand GoToAddItemCommand { get; set; }

        public MyRiskFactorsCardViewModel()
        {
            anamnesisService = DependencyService.Resolve<AnamnesisService>();
            GoToAddItemCommand = new Command(() => GoToDetail());
        }
        public async Task GoToDetail()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new MyRiskFactorDetailPage());
        }


    }
}
