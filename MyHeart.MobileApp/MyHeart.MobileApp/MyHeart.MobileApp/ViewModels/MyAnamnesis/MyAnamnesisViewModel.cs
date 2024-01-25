using MyHeart.DTO.User;
using MyHeart.MobileApp.ViewModels;
using MyHeart.MobileApp.ViewModels.MyAnamnesis.MyAlergy;
using MyHeart.MobileApp.Views.Controls;
using MyHeart.MobileApp.Views.MyAnamnesis.MyAllergy;
using MyHeart.MobileApp.Views.MyAnamnesis;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public class MyAnamnesisViewModel : BaseViewModel
    {
        public NonpharmaticTherapyCardViewModel NonpharmaticTherapyVm { get; }
        public MyTreatmentsCardViewModel MyThreatmentsVm { get; }
        public MyIllnessCardViewModel MyIllnessCardVm { get; }
        public MyAlergyCardViewModel MyAllergyCardVm { get; }
        public MyRiskFactorsCardViewModel MyRiskFactorCardVm { get; }
        public PhysicalCardViewModel PhysicalCardVm { get; }
        public MyAnamnesisViewModel()
        {
            NonpharmaticTherapyVm = new NonpharmaticTherapyCardViewModel();
            MyThreatmentsVm = new MyTreatmentsCardViewModel();
            MyIllnessCardVm = new MyIllnessCardViewModel();
            MyAllergyCardVm = new MyAlergyCardViewModel();
            MyRiskFactorCardVm = new MyRiskFactorsCardViewModel();
            PhysicalCardVm = new PhysicalCardViewModel();
        }
    }
}
