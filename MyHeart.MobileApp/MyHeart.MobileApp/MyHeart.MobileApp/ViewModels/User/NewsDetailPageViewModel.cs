using MyHeart.DTO.ProfessionalInformation;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels.User
{
    public class NewsDetailPageViewModel : BaseViewModel
    {
        private TherapyNewsDTO therapyNews;

        public ICommand GoBackCommand { get; set; }

        public TherapyNewsDTO TherapyNews
        {
            get => therapyNews;
            set => SetProperty(ref therapyNews, value);
        }

        public NewsDetailPageViewModel(TherapyNewsDTO therapyNews)
        {
            TherapyNews = therapyNews;

            GoBackCommand = new Command(() => Application.Current.MainPage.Navigation.PopModalAsync());
        }
    }
}
