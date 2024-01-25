using MyHeart.DTO;
using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public class AddMyIllnessPageViewModel : AnamnesisAddPageViewModel<UserDiseaseViewModel>
    {
        AnamnesisService anamnesisService;
        DiseaseService diseaseService;
        public AddMyIllnessPageViewModel()
        {
            anamnesisService = DependencyService.Resolve<AnamnesisService>();
            diseaseService = DependencyService.Resolve<DiseaseService>();

            _ = LoadData();
        }

        public override async Task LoadData()
        {
            IsBusy = true;

            loadMoreEnabled = true;

            //request = new DataTableRequest
            //{
            //    Page = DefaulPage,
            //    PageSize = DefaultPageSize,
            //    Filter = ""
            //};

            var diseases = await anamnesisService.GetAllDiseases();
            if (diseases != null)
            {
                //itemsTotalCountDb = diseaseTable.TotalCount;
                Items = new ObservableCollection<UserDiseaseViewModel>(diseases.Select(d => new UserDiseaseViewModel(d)));
            }

            IsBusy = false;
        }

        public override async Task AddItem(UserDiseaseViewModel item)
        {
            item.UserId = userService.UserId;

            var disease = await anamnesisService.SaveDiseaseAnamnesis(item);

            if (disease != null)
            {
                MessagingCenter.Instance.Send(this, "DiseaseAnamnesisCreated", disease);
                GoBackCommand?.Execute(null);
                NotifyCreated();
            }
            else
            {
                NotifyCreateFailed();
            }
        }

        public override async Task FilterItems()
        {
            IsBusy = true;

            //request.Filter = SearchQuery;
            //request.Page = DefaulPage;

            var diseases = await anamnesisService.GetDiseasesByName(SearchQuery);

            if (diseases != null)
            {
                Items = new ObservableCollection<UserDiseaseViewModel>(diseases.Select(d => new UserDiseaseViewModel(d)));
            }

            IsBusy = false;
        }

        public override async Task LoadMore()
        {
            if (loadMoreEnabled)
            {
                loadMoreEnabled = false;

                if (Items.Count < itemsTotalCountDb)
                {
                    request.Page = request.Page + 1;
                    var diseasetable = await diseaseService.GetDiseasesTable(request);
                    if (diseasetable != null)
                    {
                        itemsTotalCountDb = diseasetable.TotalCount;
                        foreach (var item in diseasetable.Data.ToList())
                        {
                            Items.Add(new UserDiseaseViewModel(item));
                        }

                    }
                }

                loadMoreEnabled = true;
            }
        }
    }
}
