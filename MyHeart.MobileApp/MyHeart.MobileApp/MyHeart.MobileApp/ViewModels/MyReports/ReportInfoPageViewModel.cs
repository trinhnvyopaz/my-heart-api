using MyHeart.DTO.User;
using MyHeart.MobileApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using MyHeart.MobileApp.Services;
using Xamarin.Forms;
using System.Windows.Input;
using System.IO;
using Xamarin.Essentials;

namespace MyHeart.MobileApp.Views
{
    internal class ReportInfoPageViewModel : BaseViewModel
    {
        public string Title { get; }
        public string ReportTitle { get; }
        public string ReportDescription { get; }
        //public UserReportFileDTO[] fileDTOs;
        public ObservableCollection<ReportFileViewModel> ReportFiles { get; } = new ObservableCollection<ReportFileViewModel>();
        private ReportViewModel reportViewModel;
        public readonly ReportViewModel ReportVM;
        private UserReportDTO userReportDTO;
        //private UserReportDTO report;
        public Func<Task<IEnumerable<UserReportFileDTO>>> GetFiles { get; }
        public Func<Task<bool>> DeleteReport { get; }
        public Func<UserReportUpdateDTO, Task<bool>> UpdateReport { get; }

        private readonly Action removeCurrentFromReports;
        private readonly Func<Task> updateReports;

        public ICommand ShowFileCommand { get; }

        public ReportInfoPageViewModel(ReportViewModel reportVM, UserService userService)
        {
            this.ReportVM = reportVM;
            int reportId = reportVM.DTO.Id;
            GetFiles = () => userService.GetReportFiles(reportId);
            DeleteReport = () => userService.DeleteReport(reportId);
            UpdateReport = (newReportDTO) => userService.UpdateReport(reportId, newReportDTO);
            this.removeCurrentFromReports = removeCurrentFromReports;
            this.updateReports = updateReports;

            Title = "Informace";
            ReportTitle = reportVM.Title;
            ReportDescription = reportVM.Description;
            LoadFiles();


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
        public async Task LoadFiles()
        {
            IsBusy = true;
            var fileDTOs = await GetFiles();

            foreach (var file in fileDTOs)
            {
                ReportFiles.Add(new ReportFileViewModel()
                {
                    Name = file.Name,
                    Extension = file.Extension,
                    Content = file.Content
                });
            }
            IsBusy = false;
        }
    }
}