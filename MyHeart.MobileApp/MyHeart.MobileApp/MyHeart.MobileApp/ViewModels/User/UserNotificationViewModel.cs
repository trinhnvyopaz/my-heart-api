using MyHeart.DTO.ProfessionalInformation;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.Views.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels.User
{
    public class UserNotificationViewModel : BaseViewModel
    {
        private TherapyNewsService newsService;
        private ObservableCollection<TherapyNewsDTO> news;

        public ObservableCollection<TherapyNewsDTO> News
        {
            get => news;
            set => SetProperty(ref news, value);
        }

        public ICommand GoBackCommand { get; set; }
        public ICommand GoToNotificationSettingsCommand { get; set; }
        public ICommand GoToDetailCommand { get; set; }

        public UserNotificationViewModel()
        {
            newsService = DependencyService.Resolve<TherapyNewsService>();

            GoBackCommand = new Command(() => App.Current.MainPage.Navigation.PopAsync());
            GoToNotificationSettingsCommand = new Command(() => App.Current.MainPage.Navigation.PushAsync(new NotificationSubscriptionPage()));
            GoToDetailCommand = new Command<TherapyNewsDTO>((news) => GoToDetail(news));

            _ = LoadData();
        }

        private async void GoToDetail(TherapyNewsDTO news)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new NewsDetailPage(news));
        }

        private async Task LoadData()
        {
            IsBusy = true;

            var news = await newsService.GetTherapyNews();
            if(news != null)
            {
                News = new ObservableCollection<TherapyNewsDTO>(news);
            }

            IsBusy = false;
        }
    }
}
