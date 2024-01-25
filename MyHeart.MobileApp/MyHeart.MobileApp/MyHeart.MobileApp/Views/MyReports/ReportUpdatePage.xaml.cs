using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MyHeart.DTO.User;
using MyHeart.MobileApp.ViewModels;
using System.Collections.ObjectModel;
using MyHeart.MobileApp.Services;
using System.IO;
using Xamarin.Essentials;

namespace MyHeart.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportUpdatePage : ContentPage
    {
        private readonly UserService _userService;
        Func<Task> UpdateReports;

        UpdateReportPageViewModel vm;
        private List<ReportFileViewModel> ReportFiles;
        public ReportUpdatePage( ReportViewModel reportVM,
            //ObservableCollection<ReportFileViewModel> reportFiles, 
            Func<Task> updateReports, UserService userService)
        {
            //Report = report;
            //ReportVM = reportVM;
            //ReportFiles = reportFiles.ToList();
            //UpdateReport = updateReport;
            UpdateReports = updateReports;
            _userService = userService;

            InitializeComponent();
            vm = new UpdateReportPageViewModel(reportVM, updateReports, userService);
            BindingContext = vm;

            //titleEntry.Text = ReportVM.Title;
            //descriptionEditor.Text = ReportVM.Description;

            //lastUpdateLabel.Text = $"Poslední aktualizace: {DateFormatter.GetDateTime(reportVM.LastUpdate)}";
            //reportDateLabel.Text = $"Dokument odevzdán: {DateFormatter.GetDate(reportVM.CreationDate)}";
            //typeLabel.Text = $"Typ dokumentu: {ReportTypeFunc.ToString(reportVM.ReportType)}";

            //reportFiles = new ObservableCollection<ReportFileViewModel>();

            //BindableLayout.SetItemsSource(filesLayout, reportFiles);
            //foreach ( var file in ReportFiles)
            //{
            //    reportFiles.Add(file);
            //}
        }

        private async void UpdateButton_Clicked(object sender, EventArgs e)
        {
            bool success;
            string reason;
            (success, reason) = await vm.UpdateReport();
            await DisplayAlert( success ? "Úspěch" : "Chyba", reason, "OK");
            if (success)
            {
                UpdateReports();
                Navigation.PopAsync(animated: true);
            }
            //int reportId = vm.ReportVM.DTO.Id;
            //var reportUpdate = new UserReportUpdateDTO()
            //{
            //    ReportId = reportId,
            //    ReportType = vm.Type,
            //    CreationDate = vm.ReportDate,
            //    LastUpdate = DateTime.UtcNow,
            //    Title = titleEntry.Text.Trim(),
            //    Description = descriptionEditor.Text.Trim(),
            //};

            //if (Validate(reportUpdate))
            //{
            //    bool success = await _userService.UpdateReport(reportId, reportUpdate);
            //    if (success)
            //    {
            //        await DisplayAlert("Úspěch", "Změny byli uložené.", "OK");
            //        UpdateReports();
            //        await Navigation.PopToRootAsync(animated: true);
            //    }
            //    else
            //    {
            //        await DisplayAlert("Chyba", "Změny nebyli uložené.", "OK");
            //    }
            //}
            //else
            //{
            //    await DisplayAlert("Chyba změny", "Změna není validní.", "OK");
            //}
        }

        private async void ShareFileButton_Clicked(object sender, EventArgs e)
        {
            // programmed this way so it will work even in case of multiple files in one report
            var imgBtn = (ImageButton)sender;
            var layout = (StackLayout)imgBtn.Parent;
            var label = (Label)layout.Children[0];
            var fileVM = vm.ReportFiles.First(file => file.Name == label.Text);

            var shareFile = Path.Combine(FileSystem.CacheDirectory, fileVM.Name);
            await File.WriteAllBytesAsync(shareFile, fileVM.Content);

            await Share.RequestAsync(new ShareFileRequest()
            {
                Title = "Zdílet soubor",
                File = new ShareFile(shareFile)
            });
        }

        private bool Validate(UserReportUpdateDTO reportUpdate) =>
            reportUpdate.Title != null
            && reportUpdate.Title.Length > 0
            && reportUpdate.Description != null;

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Odstranění zprávy", "Chcete odstranit tuto zprávu?", "Ano", "Ne");
            if (answer)
            {
                var success = await _userService.DeleteReport(vm.ReportVM.DTO.Id);
                if (success)
                {
                    await DisplayAlert("Úspěch", "Zpráva byla odstráněna.", "OK");
                    UpdateReports();
                    await Navigation.PopAsync();
                }
                else
                {
                    DisplayAlert("Neúspěch", "Zpráva nebyla odtráněna.", "OK");
                }
            }
        }
    }
}