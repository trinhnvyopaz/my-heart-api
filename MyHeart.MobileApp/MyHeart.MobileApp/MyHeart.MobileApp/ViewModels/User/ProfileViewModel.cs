using MyHeart.DTO.User;
using MyHeart.MobileApp.Enums;
using MyHeart.MobileApp.Models;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.Views;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private UserService userService;
        private DoctorService doctorService;
        private SessionService sessionService;
        private IToastService toastService;
        private IHealthKitService healthKitService;
        private AnamnesisService anamnesisService;
        private UserDetailViewModel user;
        private DoctorDetailViewModel doctor;
        private InsuranceCompany selectedInsuranceCompany;
        private bool biometricAutheticationEnabled;
        private bool googleFitEnabled;
        private bool healtkitEnabled;

        public ICommand SaveProfileCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand GoBackCommand { get; set; }

        public string CurrentPlatform => Device.RuntimePlatform;
        public bool GoogleFitEnabled
        {
            get => googleFitEnabled;
            set
            {
                _ = SetGoogleFitEnabledValue(googleFitEnabled, value);
            }
        }

        private async Task SetGoogleFitEnabledValue(bool oldValue, bool newValue)
        {
            bool? result;

            if (oldValue != newValue)
            {
                if (newValue)
                {
                    result = await healthKitService.ConnectToAccount();
                }
                else
                {
                    result = healthKitService.Disconnect();
                }

                if (result == true)
                {
                    googleFitEnabled = newValue;
                }
                else
                {
                    googleFitEnabled = oldValue;
                }

                OnPropertyChanged(nameof(GoogleFitEnabled));

                if (newValue)
                {
                    _ = SyncHealthData();
                }
            }
        }

        private async Task SyncHealthData()
        {
            var averageHeartRate = await healthKitService.FetchPreviousDayAverageHeartRate();
            if (averageHeartRate != null)
            {
                var yesterday = DateTime.Now.AddDays(-1).Date;
                var midYesterday = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 12, 0, 0);

                var personalAnamnesis = new PersonalRecordViewModel
                {
                    Date = midYesterday,
                    Value = averageHeartRate?.ToString(),
                    Type = AnamnesisType.HeartRate
                };

                await anamnesisService.SavePersonalAsync(personalAnamnesis);
            }

            var averageBodyMass = await healthKitService.FetchPreviousDayAverageBodyMass();
            if (averageBodyMass != null)
            {
                var yesterday = DateTime.Now.AddDays(-1).Date;
                var midYesterday = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 12, 0, 0);

                var personalAnamnesis = new PersonalRecordViewModel
                {
                    Date = midYesterday,
                    Value = averageBodyMass?.ToString(),
                    Type = AnamnesisType.Weight
                };

                await anamnesisService.SavePersonalAsync(personalAnamnesis);
            }
        }

        public bool HealthKitEnabled
        {
            get => healtkitEnabled;
            set
            {
                if (SetProperty(ref healtkitEnabled, value) && value == true)
                {
                    _ = SyncHealthData();
                }
            }
        }

        public List<InsuranceCompany> InsuranceCompanies { get; set; }

        public InsuranceCompany SelectedInsuranceCompany
        {
            get => selectedInsuranceCompany;
            set
            {
                if (SetProperty(ref selectedInsuranceCompany, value))
                {
                    User.InsuranceCompanyCode = selectedInsuranceCompany.Code;
                }
            }
        }

        public UserDetailViewModel User
        {
            get => user;
            set => SetProperty(ref user, value);
        }
        public DoctorDetailViewModel Doctor
        {
            get => doctor;
            set => SetProperty(ref doctor, value);
        }
        public bool BiometricAutheticationEnabled
        {
            get => biometricAutheticationEnabled;
            set
            {
                if (SetProperty(ref biometricAutheticationEnabled, value) && value)
                {
                    ToggleBiometricAuthetication();
                }
            }
        }

        public UserDTO CurrentUser { get; set; }

        public ProfileViewModel()
        {
            userService = DependencyService.Resolve<UserService>();
            doctorService = DependencyService.Resolve<DoctorService>();
            toastService = DependencyService.Get<IToastService>();
            healthKitService = DependencyService.Get<IHealthKitService>();
            anamnesisService = DependencyService.Resolve<AnamnesisService>();

            CurrentUser = userService.User;

            SaveProfileCommand = new Command(() => SaveProfile());
            RefreshCommand = new Command(() => LoadData());

            sessionService = DependencyService.Resolve<SessionService>();
            LogoutCommand = new Command(() =>
            {
                sessionService.StopSession();
                SecureStorage.Remove("username");
                SecureStorage.Remove("password");
                SecureStorage.Remove("biometricAutheticationEnabled");
                SecureStorage.Remove("mfasecret");

                userService.User = null;
                userService.UserId = 0;

                NavigationCallBack.Callback = null;

                Application.Current.MainPage = new NavigationPage(new LoginPage());
            });

            GoBackCommand = new Command(() => App.Current.MainPage.Navigation.PopAsync());

            InitInsuranceCompanies();
            _ = GetBiometricAutheticationSetting();

            _ = LoadData();
        }

        private async Task ToggleBiometricAuthetication()
        {
            bool isAvailable = await CrossFingerprint.Current.IsAvailableAsync();

            if (isAvailable)
            {
                var result = await CrossFingerprint.Current.AuthenticateAsync(new AuthenticationRequestConfiguration("Naskenujte otisk!", "Pro povolení přihlašování pomocí biometrických sensoru"));
                if (result.Authenticated)
                {
                    await SecureStorage.SetAsync("biometricAutheticationEnabled", BiometricAutheticationEnabled.ToString());
                }
                else
                {
                    biometricAutheticationEnabled = !BiometricAutheticationEnabled;
                    OnPropertyChanged(nameof(BiometricAutheticationEnabled));
                }
            }
            else
            {
                base.toastService.ShowError("Biometrícké přihlašování není dostupné");
                biometricAutheticationEnabled = !BiometricAutheticationEnabled;
                OnPropertyChanged(nameof(BiometricAutheticationEnabled));
            }
        }

        private async Task GetBiometricAutheticationSetting()
        {
            var setting = await SecureStorage.GetAsync("biometricAutheticationEnabled");
            if (setting != null)
            {
                biometricAutheticationEnabled = Convert.ToBoolean(setting);
            }
        }
        private void InitInsuranceCompanies()
        {
            InsuranceCompanies = new List<InsuranceCompany>
            {
                new InsuranceCompany{ Name = "Česká průmyslová zdravotní pojišťovna", Code = 205 },
                new InsuranceCompany{ Name = "Oborová zdravotní pojišťovna zaměstnanců bank, pojišťoven a stavebnictví", Code = 207 },
                new InsuranceCompany{ Name = "RBP, zdravotní pojišťovna", Code = 213 },
                new InsuranceCompany{ Name = "Všeobecná zdravotní pojišťovna České republiky", Code = 111 },
                new InsuranceCompany{ Name = "Vojenská zdravotní pojišťovna ČR", Code = 201 },
                new InsuranceCompany{ Name = "Zaměstnanecká pojišťovna Škoda", Code = 209 },
                new InsuranceCompany{ Name = "Zdravotní pojišťovna ministerstva vnitra", Code = 211 }
            };
        }
        private async Task SaveProfile()
        {
            IsBusy = true;

            var result = false;

            if (CurrentUser.UserType == UType.Doctor)
            {
                result = await doctorService.UpdateAsync(Doctor);
            }
            else
            {
                result = await userService.UpdateAsync(User);
            }

            if (result)
            {
                await ReloadUser();
                toastService.ShowSuccess("Uloženo");
            }
            else
            {
                toastService.ShowError("Nepodařilo se uložit profil");
            }

            IsBusy = false;
        }

        private async Task ReloadUser()
        {
            var currentUser = await userService.GetUserDetailAsync();
            User = new UserDetailViewModel(currentUser);
            SelectedInsuranceCompany = InsuranceCompanies.FirstOrDefault(i => i.Code == User.InsuranceCompanyCode);
        }

        private async Task LoadData()
        {
            IsBusy = true;

            var currentUser = userService.User;

            if (currentUser.UserType == UType.Doctor)
            {
                var doctorResponse = await doctorService.GetDoctorDetailAsync(currentUser.Id);

                Doctor = new DoctorDetailViewModel(doctorResponse);
            }
            else
            {
                var userResponse = await userService.GetUserDetailAsync();

                if (userResponse != null)
                {
                    User = new UserDetailViewModel(userResponse);
                    SelectedInsuranceCompany = InsuranceCompanies.FirstOrDefault(i => i.Code == User.InsuranceCompanyCode);
                }
            }



            var heathTrackingServiceEnabled = healthKitService.IsEnabled();

            if (Device.RuntimePlatform == Device.Android)
            {
                googleFitEnabled = heathTrackingServiceEnabled;
                OnPropertyChanged(nameof(GoogleFitEnabled));
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                healtkitEnabled = heathTrackingServiceEnabled;
                OnPropertyChanged(nameof(HealthKitEnabled));
            }

            IsBusy = false;
        }
    }
}
