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

namespace MyHeart.MobileApp.ViewModels
{
    public class UpdateReportPageViewModel : BaseViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ReportTypeDTO Type { get; set; }
        public object[] TypeSource { get => ReportTypeFunc.pairs.Select(p => p.Item2).ToArray(); }
        public string ReportType { get => $"{ReportTypeFunc.ToString(Type)}"; set => Type = ReportTypeFunc.pairs.First(rt => rt.Item2 == value).Item1; }
        public string ReportTypeFormatted { get => $"Typ dokumentu: {ReportTypeFunc.ToString(Type)}"; }
        public DateTime LastUpdate { get; }
        public string LastUpdateFormatted { get => $"Poslední aktualizace: {LastUpdate}"; }
        public DateTime ReportDate { get; set; }
        public string ReportDateFormatted { get => $"Dokument odevzdán: {ReportDate}"; }
        public ObservableCollection<ReportFileViewModel> ReportFiles { get; } = new ObservableCollection<ReportFileViewModel>();
        public readonly ReportViewModel ReportVM;
        public Command UpdateReportCommand;
        private Func<Task<IEnumerable<UserReportFileDTO>>> LoadReportFiles;
        private readonly int reportId;
        private readonly UserService _userService;
        private readonly Func<Task> UpdateReports;

        public ICommand ShowFileCommand { get; }

        public UpdateReportPageViewModel(ReportViewModel reportVM,
            //ObservableCollection<ReportFileViewModel> reportFiles,
            Func<Task> updateReports, UserService userService)
        {
            _userService = userService;
            UpdateReports = updateReports;
            ReportVM = reportVM;
            Title = reportVM.Title;
            Description = reportVM.Description;
            //string lastUpdateStr = DateFormatter.GetDateTime(reportVM.LastUpdate);
            LastUpdate = reportVM.LastUpdate;
            //string reportDateStr = DateFormatter.GetDate(reportVM.CreationDate);
            ReportDate = reportVM.CreationDate;
            Type = reportVM.ReportType;
            LoadReportFiles = async () => await userService.GetReportFiles(reportVM.DTO.Id);
            LoadFiles();
            //ReportFiles = new List<ReportFileViewModel>(reportFiles);
            reportId = reportVM.DTO.Id;


            UpdateReportCommand = new Command(async () => {
                var reportUpdate = new UserReportUpdateDTO()
                {
                    ReportType = Type,
                    ReportId = reportId,
                    Title = Title.Trim(),
                    CreationDate = ReportDate,
                    LastUpdate = DateTime.UtcNow,
                    Description = Description.Trim()
                };
                if (Validate(reportUpdate))
                {
                    IsBusy = true;
                    bool success = await userService.UpdateReport(reportId, reportUpdate);
                    if (success)
                    {
                        await Application.Current.MainPage.DisplayAlert("Úspěch", "Změny byli uložené.", "OK");
                        updateReports();
                        await Application.Current.MainPage.Navigation.PopToRootAsync(animated: true);
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Chyba", "Změny nebyli uložené.", "OK");
                    }
                    IsBusy = false;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Chyba změny", "Změna není validní.", "OK");
                }
            });


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
        }

        private async Task LoadFiles()
        {
            IsBusy = true;
            var reportFiles = await LoadReportFiles();
            foreach (var reportFile in reportFiles)
            {
                ReportFiles.Add( new ReportFileViewModel()
                {
                    Name = reportFile.Name,
                    Extension = reportFile.Extension,
                    Content = reportFile.Content
                });
            }
            IsBusy = false;
            //ReportFiles = new List<ReportFileViewModel>(reportFiles);
        }
        public async Task<(bool success, string reason)> UpdateReport()
        {
            var reportUpdate = new UserReportUpdateDTO()
            {
                ReportType = Type,
                ReportId = reportId,
                Title = Title.Trim(),
                CreationDate = ReportDate,
                LastUpdate = DateTime.UtcNow,
                Description = Description.Trim()
            };

            IsBusy = true;
            (bool success, string reason) res = (false, string.Empty);
            if (Validate(reportUpdate))
            {
                IsBusy = true;
                bool success = await _userService.UpdateReport(reportId, reportUpdate);
                if (success)
                {
                    //UpdateReports();
                    res.success = true;
                    res.reason = "Změny byli uložené.";
                }
                else
                {
                    res.reason = "Změny nebyli uložené.";
                }
                //return res;
            }
            else
            {
                res.reason = "Změna není validní.";
            }
            IsBusy = true;
            return res;
        }
        private bool Validate(UserReportUpdateDTO reportUpdate) =>
            reportUpdate.Title != null
            && reportUpdate.Title.Length > 0
            && reportUpdate.Description != null;
    }
}
