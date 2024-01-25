using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels.MyReports
{
    public class ReportDetailViewModel : BaseViewModel
    {
        private ReportViewModel report;
        private UserService userService;
        private ReportFileViewModel reportFile;

        public ICommand GoBackCommand { get; set; }
        public ICommand DeleteReportCommand { get; set; }
        public ICommand ShowFileCommand { get; set; }
        public ICommand UpdateReportCommand { get; set; }

        public object[] TypeSource { get => ReportTypeFunc.pairs.Select(p => p.Item2).ToArray(); }
        // SelectedType is string really
        public object SelectedType { get; set; }

        public ReportViewModel Report
        {
            get => report;
            set => SetProperty(ref report, value);
        }

        public ReportFileViewModel ReportFile
        {
            get => reportFile;
            set => SetProperty(ref reportFile, value);
        }

        public ReportDetailViewModel(ReportViewModel report)
        {
            Report = report;
            SelectedType = ReportTypeFunc.pairs.FirstOrDefault(x => x.Item1 == report.ReportType).Item2;
            userService = DependencyService.Resolve<UserService>();

            GoBackCommand = new Command(GoBack);
            DeleteReportCommand = new Command(() => DeleteReport());
            UpdateReportCommand = new Command(UpdateReport);

            ShowFileCommand = new Command<ReportFileViewModel>(async file =>
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string filename = Path.Combine(path, file.Name);

                await File.WriteAllBytesAsync(filename, file.Content);

                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filename)
                });

                File.Delete(filename);
            });

            LoadFiles();
        }

        private async void UpdateReport()
        {
            var dto = new UserReportUpdateDTO
            {
                ReportId = report.Id,
                Title = report.Title,
                Description = report.Description,
                ReportType = ReportTypeFunc.pairs.FirstOrDefault(x => x.Item2 == (string)SelectedType).Item1 
            };

            var result = await userService.UpdateReport(Report.Id, dto);
            if(result)
            {
                toastService.ShowSuccess("Dokument uložen");
                MessagingCenter.Send(this, "ReportUpdated");
            }
            else
            {
                toastService.ShowError("Nepodařilo se uložit dokument");
            }
        }

        private async Task LoadFiles()
        {
            var files = await userService.GetReportFiles(Report.Id);
            if(files != null && files.Count() > 0)
            {
                var file = files.First();
                ReportFile = new ReportFileViewModel(file);
            }
        }

        private async Task DeleteReport()
        {
            IsBusy = true;

            var result = await userService.DeleteReport(Report.Id);
            if (result)
            {
                toastService.ShowSuccess("Dokument úspěšně smazán");
                MessagingCenter.Send(this, "ReportDeleted");
                GoBack();
            }
            else
            {
                toastService.ShowError("Nepodařilo se smazat dokument");
            }

            IsBusy = false;
        }

        private async void GoBack()
        {
            await App.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
