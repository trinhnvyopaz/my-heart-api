using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.Views.MyReports;
using NativeMedia;
using Plugin.Permissions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing;

namespace MyHeart.MobileApp.ViewModels.MyReports
{
    public class ReportAddFilePageViewModel : BaseViewModel
    {
        private readonly IVisionService visionService;
        public readonly UserService userService;
        private readonly INavigation navigation;
        private bool showFilePickerAppDialog;

        public ICommand TakePhotoCommand { get; }
        public ICommand ScanPhotoIOSCommand { get; }
        public ICommand PickFileCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand PickFileiOSCommand { get; }
        public ICommand PickPhotoiOSCommand { get; }
        public ICommand ClosePickerAppDialogCommand { get; }

        public bool ShowFilePickerAppDialog
        {
            get => showFilePickerAppDialog;
            set => SetProperty(ref showFilePickerAppDialog, value);
        }

        public ReportAddFilePageViewModel(INavigation navigation)
        {
            this.navigation = navigation;

            userService = DependencyService.Resolve<UserService>();
            visionService = DependencyService.Get<IVisionService>();

            TakePhotoCommand = new Command(() => _ = TakePhoto());
            PickFileCommand = new Command(() => _ = CheckPermissionAndPickFile());
            PickFileiOSCommand = new Command(() => _ = PickFile());
            PickPhotoiOSCommand = new Command(() => _ = PickPhoto());
            ScanPhotoIOSCommand = new Command(() => _ = ScanPhoto());
            ClosePickerAppDialogCommand = new Command(() => ShowFilePickerAppDialog = false);
            GoBackCommand = new Command(() => navigation.PopAsync());

            MessagingCenter.Subscribe<ReportAddInfoPageViewModel, UserReportDTO>(this, "ReportUploaded", (sender, item) => App.Current.MainPage.Navigation.PopModalAsync());
        }

        private async Task PickPhoto()
        {
            ShowFilePickerAppDialog = false;

            IMediaFile[] files;

            try
            {
                var status = await Permissions.RequestAsync<Permissions.StorageRead>();

                if (status != PermissionStatus.Granted)
                    return;


                var results = await MediaGallery.PickAsync(1, MediaFileType.Image);
                files = results?.Files?.ToArray();

                if (files != null)
                {
                    var file = files[0];
                    var ms = await file.OpenReadAsync();
                    ReportFileViewModel reportFileVM = GetFileFromMemoryStream(ms, file);
                    await navigation.PushModalAsync(new ReportAddInfoPage(reportFileVM));
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private async Task ScanPhoto()
        {
            var fileVM = await TakePhoto();
            if (fileVM != null)
            {
                var imageStream = visionService.DetectAndCropDocuments(fileVM.Content);

                if (imageStream != null)
                {
                    var memoryStream = new MemoryStream();
                    await imageStream.CopyToAsync(memoryStream);
                    fileVM = GetFileFromMemoryStream(memoryStream, fileVM.Name);

                }
                
                await App.Current.MainPage.Navigation.PushAsync(new ReportAddInfoPage(fileVM));              
            }
        }

        private async Task CheckPermissionAndPickFile()
        {
            ShowFilePickerAppDialog = false;

            if (Device.RuntimePlatform == Device.iOS)
            {
                ShowFilePickerAppDialog = true;
            }
            else
            {
                await PickFile();
            }
        }

        public async Task PickFile()
        {
            ShowFilePickerAppDialog = false;

            var options = new PickOptions
            {
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new [] { "com.adobe.pdf", "public.image" } },
                    { DevicePlatform.Android, new [] { "application/pdf", "image/*" } }
                })
            };

            var result = await FilePicker.PickAsync(options);

            if (result != null)
            {
                string fileName = result.FileName;
                var ms = await result.OpenReadAsync();
                ReportFileViewModel reportFileVM = GetFileFromMemoryStream(ms, fileName);
                await navigation.PushModalAsync(new ReportAddInfoPage(reportFileVM));

            }

            ShowFilePickerAppDialog = false;
        }

        private async Task<ReportFileViewModel> TakePhoto()
        {
            if (MediaGallery.CheckCapturePhotoSupport())
            {
                var status = await Permissions.RequestAsync<Permissions.Camera>();

                if (status != PermissionStatus.Granted)
                    return null;

                try
                {
                    var result = await MediaGallery.CapturePhotoAsync();

                    if (result is null)
                        return null;

                    var ms = await result.OpenReadAsync();
                    ReportFileViewModel reportFileVM = GetFileFromMemoryStream(ms, result);

                    await navigation.PushModalAsync(new ReportAddInfoPage(reportFileVM));

                    return reportFileVM;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Není podporováno", "Na vašem zařízení není podporována tato funkcionalita", "OK");
                return null;

            }
        }

        private ReportFileViewModel GetFileFromMemoryStream(Stream stream, IMediaFile file)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var report = new ReportFileViewModel()
                {
                    Name = file.NameWithoutExtension,
                    Extension = file.Extension,
                    Content = ms.ToArray()
                };

                return report;
            }
        }

        private ReportFileViewModel GetFileFromMemoryStream(Stream stream, string fileName)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var extension = Path.GetExtension(fileName);
                var report = new ReportFileViewModel()
                {
                    Name = fileName,
                    Extension = extension,
                    Content = ms.ToArray()
                };

                return report;
            }
        }
    }
}
