using MyHeart.DTO.ProfessionalInformation;
using MyHeart.MobileApp.Models;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.Views.PopUp;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public class RecomandationsViewModel : BaseViewModel
    {
        private AnamnesisService anamnesisService;
        private ObservableCollection<string> recommandations;

        private string searchQuery;
        double searchQueryInterval = 500;
        Timer searchQueryTimer;

        public ObservableCollection<string> Recommandations
        {
            get => recommandations;
            set => SetProperty(ref recommandations, value); 
        }

        public string SearchQuery
        {
            get => searchQuery;
            set => SetProperty(ref searchQuery, value);
        }

        public RecomandationsViewModel()
        {
            anamnesisService = DependencyService.Resolve<AnamnesisService>();


            SearchQuery = "";

            searchQueryTimer = new Timer(searchQueryInterval);
            searchQueryTimer.Elapsed += SearchQueryTimer_Elapsed;


            _ = LoadDataAsync();
        }

        private void SearchQueryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            searchQueryTimer.Stop();
            searchQueryTimer.Enabled = false;
            //FilterItems();
        }

        //public override async Task FilterItems()
        //{
            //IsBusy = true;

            //request.Filter = SearchQuery;
            //request.Page = DefaulPage;

            //var diseases = await anamnesisService.GetDiseaseAnamneses(SearchQuery);

            //if (diseases != null)
            //{
            //    Items = new ObservableCollection<UserDiseaseViewModel>(diseases.Select(d => new UserDiseaseViewModel(d)));
            //}

            //IsBusy = false;
        //}

        public async Task LoadDataAsync()
        {
            IsBusy = true;

            var diseases = await anamnesisService.GetDiseaseAnamneses();

            Recommandations = new ObservableCollection<string>(diseases.Select(d => d.Disease.SystemicMeasures));

            IsBusy = false;
        }
    }
}
