using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using MyHeart.MobileApp.Services;
using Xamarin.Forms;
using System.Windows.Input;
using MyHeart.MobileApp.Views.MyReports;
using MyHeart.MobileApp.Views;
using NativeMedia;

namespace MyHeart.MobileApp.ViewModels.MyReports
{
    public class ReportPageViewModel : BaseViewModel
    {
        private UserService userService;
        private ObservableCollection<ReportViewModel> reports;

        public ObservableCollection<ReportViewModel> Reports
        {
            get => reports;
            set => SetProperty(ref reports, value);
        }
        public ICommand GoToAddFileCommand { get; set; }
        public ICommand GoToReportDetailCommand { get; set; }

        public ReportPageViewModel()
        {
            Reports = new ObservableCollection<ReportViewModel>();
            userService = DependencyService.Resolve<UserService>();

            GoToAddFileCommand = new Command(() => GoToAddFile());
            GoToReportDetailCommand = new Command<int>(GoToReportDetail);

            MessagingCenter.Subscribe<ReportDetailViewModel>(this, "ReportDeleted", sender => LoadData());
            MessagingCenter.Subscribe<ReportDetailViewModel>(this, "ReportUpdated", sender => LoadData());
            MessagingCenter.Subscribe<ReportAddInfoPageViewModel, UserReportDTO>(this, "ReportUploaded", (sender, item) => LoadData());

            LoadData();
        }

        private async void GoToReportDetail(int id)
        {
            var report = reports.FirstOrDefault(x => x.Id == id);
            await App.Current.MainPage.Navigation.PushModalAsync(new ReportDetailPage
            {
                BindingContext = new ReportDetailViewModel(report)
            });
        }

        private async Task GoToAddFile()
        {
            var reportAddPage = new ReportAddFilePage();

            await App.Current.MainPage.Navigation.PushAsync(reportAddPage);
        }

        public async Task LoadData()
        {
            try
            {
                var reports = await userService.GetReports();
                if (reports != null)
                {
                    Reports = new ObservableCollection<ReportViewModel>(reports.Reverse().Select(r => new ReportViewModel
                    {
                        Id = r.Id,
                        DTO = r,
                        Title = r.Title,
                        ReportType = r.ReportType,
                        LastUpdate = r.LastUpdate,
                        Description = r.Description,
                        CreationDate = r.CreationDate,
                        Files = new List<ReportFileViewModel>
                    {
                        new ReportFileViewModel
                        {
                            Extension = r.Files?.FirstOrDefault()?.Extension
                        }
                    }
                    }));
                }
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        public async Task UpdateReports(UserService userService) //, IEnumerable<UserReportDTO> reports)
        {
            Reports.Clear();
            IsBusy = true;
            var reports = await userService.GetReports();
            // data from API come in "oldest first" order
            foreach (var report in reports.Reverse())
            {
                Reports.Add(new ReportViewModel()
                {
                    DTO = report,
                    Title = report.Title,
                    ReportType = report.ReportType,
                    LastUpdate = report.LastUpdate,
                    Description = report.Description,
                    CreationDate = report.CreationDate,
                    Files = new List<ReportFileViewModel>()
                });
            }
            IsBusy = false;

        }
    }
}
