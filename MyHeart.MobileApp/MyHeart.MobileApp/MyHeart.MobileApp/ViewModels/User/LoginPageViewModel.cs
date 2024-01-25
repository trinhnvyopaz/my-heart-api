using Microsoft.AppCenter.Crashes;
using MyHeart.DTO;
using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.Views;
using NativeMedia;
using Newtonsoft.Json;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Plugin.FirebasePushNotification;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Mobile;

namespace MyHeart.MobileApp.ViewModels
{

    public class LoginPageViewModel : BaseViewModel
    {
        private readonly LoginService loginService;
        private readonly UserService userService;
        private readonly IHealthKitService healthKitService;
        private readonly SessionService sessionService;

        private LoginViewModel login;
        private bool mfaNeeded;
        private bool showBiometricAuthetication;
        private AuthenticationType biometricType;
        private bool _isSigningViaBiometrics;

        public ICommand GoToRegistration { get; }
        public ICommand SignInCommand { get; }
        public ICommand BiometricAutheticationCommand { get; }

        public LoginViewModel Login
        {
            get => login;
            set
            {
                if (SetProperty(ref login, value))
                {
                    //login.Username.
                }
            }

        }
        public bool IsSigningViaBiometrics
        {
            get => _isSigningViaBiometrics;
            set => SetProperty(ref _isSigningViaBiometrics, value);
        }
        public bool ShowBiometricAuthetication
        {
            get => showBiometricAuthetication;
            set => SetProperty(ref showBiometricAuthetication, value);
        }
        public bool MfaNeeded
        {
            get => mfaNeeded;
            set => SetProperty(ref mfaNeeded, value);
        }
        public AuthenticationType BiometricType
        {
            get => biometricType;
            set => SetProperty(ref biometricType, value);
        }
        public LoginPageViewModel()
        {
            loginService = DependencyService.Resolve<LoginService>();
            userService = DependencyService.Resolve<UserService>();

            GoToRegistration = new Command(() => Application.Current.MainPage.Navigation.PushModalAsync(new RegistrationPage()));
            SignInCommand = new Command(() => SignIn());
            BiometricAutheticationCommand = new Command(() => BiometricAuthetication());


            Login = new LoginViewModel();

#if DEBUG
            login.Username = "krystof.lach@memos.cz";
            login.Password = "123456";
#endif

            Login.PropertyChanged += Login_PropertyChanged;
            sessionService = DependencyService.Resolve<SessionService>();

            SetShowBiometricAuthetication();
        }


        private async Task SetShowBiometricAuthetication()
        {
            BiometricType = await CrossFingerprint.Current.GetAuthenticationTypeAsync();

            var setting = await SecureStorage.GetAsync("biometricAutheticationEnabled");
            var isAvailable = await CrossFingerprint.Current.IsAvailableAsync();


            if (setting != null && isAvailable)
            {
                bool isEnabled = Convert.ToBoolean(setting);
                ShowBiometricAuthetication = isEnabled;
            }
            if (ShowBiometricAuthetication)
            {
                await BiometricAuthetication();
            }
        }
        private async Task BiometricAuthetication()
        {

            var isAvailable = await CrossFingerprint.Current.IsAvailableAsync();

            if (!isAvailable)
            {
                toastService.ShowError("Biometrické přihlášení není dostupné");
                return;
            }
            string title = "Pro přihlášení naskanujte";
            if (biometricType == AuthenticationType.Face)
            {
                title += " tvář";
            }
            else
            {
                title += " prst";
            }
            var request = new AuthenticationRequestConfiguration(title, " ");
            var result = await CrossFingerprint.Current.AuthenticateAsync(request);
            if (result.Authenticated)
            {
                string username = await SecureStorage.GetAsync("user");
                string password = await SecureStorage.GetAsync("password");

                //set username/password withou notifying ui
                var login = new LoginViewModel
                {
                    Username = username,
                    Password = password,
                    RememberMe = true
                };

                IsSigningViaBiometrics = true;

                await SignIn(login);
            }
            else
            {
                Login.Username = "";
                Login.Password = "";

                toastService.ShowError("Nepodařilo se přihlásit");
            }
        }

        private void Login_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LoginViewModel.Username))
            {
                if (mfaNeeded)
                {
                    MfaNeeded = false;
                }
            }
        }

        private async Task SignIn(LoginViewModel login = null)
        {
            if (login == null)
            {
                login = Login;
            }

            login.MfaCode = Login.MfaCode;
            login.RememberMe = true;

            bool isValid = Validate(login);
            if (!isValid)
            {
                return;
            }

            IsBusy = true;

            ApiResponse<LoginResponseDTO> response = null;

            string mfaSecret = await SecureStorage.GetAsync("mfasecret");

            if (MfaNeeded || !string.IsNullOrEmpty(mfaSecret))
            {
                if (string.IsNullOrEmpty(login.MfaCode))
                {
                    login.MfaCode = mfaSecret;
                }

                response = await loginService.MfaLogin(login);
            }
            else
            {
                response = await loginService.Login(login);
            }

            if (!response.IsSuccess)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    Login.IsValid = false;
                    Login.InvalidMessage = "Špatný request";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
                    Login.IsValid = false;
                    Login.InvalidMessage = errorResponse.Title;
                    if (errorResponse.Title == "MFA bylo zadáno špatně" && !MfaNeeded)
                    {
                        SecureStorage.Remove("mfasecret");
                        SignIn(login);
                    }
                    if (errorResponse.Title == "Email nebo heslo bylo zadáno špatně")
                    {
                        IsSigningViaBiometrics = false;
                    }
                }
                else
                {
                    Login.IsValid = false;
                    Login.InvalidMessage = "Něco se pokazilo, zkontrolujte připojení k internetu";
                }

            }
            else if (response.Content.Contains(ServiceConfig.MFANeededCode))
            {
                MfaNeeded = true;
                if (IsSigningViaBiometrics)
                {
                    Login = login;
                }
            }
            else
            {
                try
                {
                    await SecureStorage.SetAsync("user", login.Username);
                    await SecureStorage.SetAsync("password", login.Password);
                    if (!String.IsNullOrEmpty(response.Data.MfaSecret))
                    {
                        await SecureStorage.SetAsync("mfasecret", response.Data.MfaSecret);
                    }
                }
                catch (Exception ex)
                {
                }

                await OnSuccusfullLogin();
            }

            IsBusy = false;
        }

        private async Task OnSuccusfullLogin()
        {
            MfaNeeded = false;

            await userService.GetCurrentAsync();
            _ = userService.AddFmcToken(CrossFirebasePushNotification.Current.Token);

            Application.Current.MainPage = new NavigationPage(new MainPage());

            sessionService.StartSession();

            IsSigningViaBiometrics = false;

            await NavigationCallBack.Callback?.Invoke();
            NavigationCallBack.Callback = null;
        }

        public bool Validate(LoginViewModel login)
        {
            Login.IsValid = true;
            Login.InvalidMessage = "";

            if (string.IsNullOrEmpty(login.Username) && string.IsNullOrEmpty(login.Password))
            {
                Login.IsValid = false;
                Login.InvalidMessage = "Vyplňte email a heslo";
            }
            if (MfaNeeded && string.IsNullOrEmpty(login.MfaCode))
            {
                Login.IsValid = false;
                Login.InvalidMessage += "\nVyplňte MFA kód";
            }

            return Login.IsValid;
        }
        public class MobileLoginVM
        {
            public string Email { get; set; }
            public string Token { get; set; }
        }
    }
}
