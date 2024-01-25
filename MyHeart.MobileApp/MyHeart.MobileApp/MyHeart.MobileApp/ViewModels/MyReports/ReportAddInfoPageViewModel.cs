using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels.MyReports
{
    public class ReportAddInfoPageViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private ReportFileViewModel _reportFile;

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public object[] TypeSource { get => ReportTypeFunc.pairs.Select(p => p.Item2).ToArray(); }
        // SelectedType is string really
        public object SelectedType { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.UtcNow.Date;

        public ICommand ShowFileCommand { get; }
        public ICommand SaveReportCommand { get; set; }
        public ICommand GoBackCommand { get; set; }
        public ReportFileViewModel ReportFile
        {
            get => _reportFile;
            set => SetProperty(ref _reportFile, value);
        }

        public ReportAddInfoPageViewModel(ReportFileViewModel file)
        {
            _userService = DependencyService.Resolve<UserService>();
            ReportFile = file;


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

            SaveReportCommand = new Command(() => _ = UploadAsync());
            GoBackCommand = new Command(() => App.Current.MainPage.Navigation.PopModalAsync());
        }
        public async Task UploadAsync()
        {
            var report = new ReportViewModel();

            report.Title = Title.Trim();
            report.Description = Description.Trim();
            report.Files = new[] { ReportFile }.ToList();
            report.LastUpdate = DateTime.UtcNow;
            report.CreationDate = CreationDate;
            report.ReportType = ReportTypeFunc.pairs.FirstOrDefault(x => x.Item2 == (string)SelectedType).Item1;


            if (ValidateReport(report))
            {
                IsBusy = true;

                var newReport = await _userService.AddReportAsync(report);
                if (newReport != null)
                {
                    toastService.ShowSuccess("Dokument byl uložený");
                    MessagingCenter.Send(this, "ReportUploaded", newReport);
                    await App.Current.MainPage.Navigation.PopModalAsync();
                }
                else
                {
                    toastService.ShowError("Nepodařilo sa přidat dokument");
                }

                IsBusy = false;
            }


        }

        bool ValidateReport(ReportViewModel rvm)
        {
            bool isValid = true;


            if (string.IsNullOrEmpty(rvm.Title))
            {
                toastService.ShowError("Není vyplněný nadpis");
                isValid = false;
            }
            else if (rvm.Description == null)
            {
                toastService.ShowError("Není vyplněný popis");
                isValid = false;
            }

            return isValid;
        }

    }
}
