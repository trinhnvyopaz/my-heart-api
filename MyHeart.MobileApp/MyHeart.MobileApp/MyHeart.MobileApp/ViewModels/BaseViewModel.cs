using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;
using MyHeart.MobileApp.Models;
using MyHeart.MobileApp.Services;
using System.Windows.Input;
using MyHeart.MobileApp.Views;
using MyHeart.MobileApp.Views.User;
using MyHeart.DTO.User;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xamarin.Essentials;
using ZXing.Mobile;

namespace MyHeart.MobileApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        private string title;
        private bool _isValid;

        public ICommand GoToProfilePageCommand { get; }
        protected IToastService toastService;
        private UserService userService;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ScanQrCodeCommand { get; }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }
        public bool IsNotBusy
        {
            get => !IsBusy;
        }

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public bool IsValid
        {
            get => _isValid;
            set => SetProperty(ref _isValid, value);
        }

        public BaseViewModel()
        {
            GoToProfilePageCommand = new Command(GoToProfilePage);
            toastService = DependencyService.Get<IToastService>();
            userService = DependencyService.Get<UserService>();
            ScanQrCodeCommand = new Command(() => ScanQrCode());
        }

        private async Task ScanQrCode()
        {
            IsBusy = true;

            var scanner = new MobileBarcodeScanner();

            var result = await scanner.Scan();

            if (result != null)
            {
                try
                {
                    //var parsedQrCode = result.Text.Replace("\\", "");
                    var phoneAuth = JsonConvert.DeserializeObject<PhoneAuthResponseVM>(result.Text);

                    var request = new PhoneAuthResponseDTO
                    {
                        AuthId = phoneAuth.Id,
                        Secret = phoneAuth.Secret
                    };

                    await userService.AuthorizeByPhone(request);

                }
                catch (Exception ex)
                {
                    toastService.ShowError("Nepadařilo se zpracovat qr kód");
                }
            }

            IsBusy = false;
        }

        private async void GoToProfilePage()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new UserMenuPage());
        }

        public void NotifyDeleted()
        {
            toastService.ShowSuccess("Vymazáno");
        }

        public void NotifyDeleteFailed()
        {
            toastService.ShowError("Nepodařilo se vymazat položku");
        }

        public void NotifyCreated()
        {
            toastService.ShowSuccess("Vytvořeno");
        }

        public void NotifyCreateFailed()
        {
            toastService.ShowError("Nepodařilo se vytvořit položku");
        }

        public void NotifyUpdated()
        {
            toastService.ShowSuccess("Uloženo");
        }

        public void NotifyUpdateFailed()
        {
            toastService.ShowError("Nepodařilo se uložit položku");
        }

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
