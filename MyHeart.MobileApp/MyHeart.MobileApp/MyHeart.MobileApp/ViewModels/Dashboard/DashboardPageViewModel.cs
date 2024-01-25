using MyHeart.DTO.User;
using MyHeart.MobileApp.Enums;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.ViewModels.MyQuestion;
using MyHeart.MobileApp.ViewModels.PopUps;
using MyHeart.MobileApp.Views;
using MyHeart.MobileApp.Views.Controls;
using MyHeart.MobileApp.Views.PopUp;
using MyHeart.MobileApp.Views.Questionnaire;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Mobile;

namespace MyHeart.MobileApp.ViewModels
{
    public class DashboardPageViewModel : BaseViewModel
    {
        private UserService userService;
        private AnamnesisService anamnesisService;
        private IHealthKitService healthKitService;
        private UserDetailDTO userDetail;

        IPopupNavigation popupNavigation = PopupNavigation.Instance;
        private UserAnamnesisDTO abususAnamnesis;
        private UserAnamnesisDTO generalAnamnesis;
        private UserAnamnesisDTO weightAnamnesis;
        private UserAnamnesisDTO bloodPresureAnamnesis;
        private UserAnamnesisDTO heartRateAnamnesis;
        private UserAnamnesisDTO cholesterolAnamnesis;

        public ICommand GotoPageCommand { get; }
        public ICommand GotoQuestionCreationCommand { get; }
        public ICommand GotoIsSmokerDetailCommand { get; }
        public ICommand GotoPersonalAnamnesisCommand { get; }
        public ICommand GotoPhysicalActivityCommand { get; }
        public ICommand WebLogInCommand { get; }
        public ICommand GotoQuestionaireCommand { get; }
        public UserDetailDTO UserDetail
        {
            get => userDetail;
            set => SetProperty(ref userDetail, value);
        }
        public UserAnamnesisDTO AbususAnamnesis
        {
            get => abususAnamnesis;
            set => SetProperty(ref abususAnamnesis, value);
        }
        public UserAnamnesisDTO GeneralAnamnesis
        {
            get => generalAnamnesis;
            set => SetProperty(ref generalAnamnesis, value);
        }
        public UserAnamnesisDTO WeightAnamnesis
        {
            get => weightAnamnesis;
            set => SetProperty(ref weightAnamnesis, value);
        }
        public UserAnamnesisDTO BloodPressureAnamnesis
        {
            get => bloodPresureAnamnesis;
            set => SetProperty(ref bloodPresureAnamnesis, value);
        }
        public UserAnamnesisDTO HeartRateAnamnesis
        {
            get => heartRateAnamnesis;
            set => SetProperty(ref heartRateAnamnesis, value);
        }
        public UserAnamnesisDTO CholesterolAnamnesis
        {
            get => cholesterolAnamnesis;
            set => SetProperty(ref cholesterolAnamnesis, value);
        }
        public DashboardPageViewModel()
        {
            userService = DependencyService.Resolve<UserService>();
            anamnesisService = DependencyService.Resolve<AnamnesisService>();
            healthKitService = DependencyService.Get<IHealthKitService>();

            GotoQuestionCreationCommand = new Command(() => GotoQuestionCreation());
            GotoIsSmokerDetailCommand = new Command(() => GotoIsSmokerDetail());
            GotoPersonalAnamnesisCommand = new Command<AnamnesisType>(type => GotoPersonalAnamnesis(type));
            GotoPhysicalActivityCommand = new Command(() => GoToPhysicalActivity());
            WebLogInCommand = new Command(() => WebLogIn());
            GotoQuestionaireCommand = new Command(() => GotoQuestionaire());

            MessagingCenter.Instance.Subscribe<BasePopUpViewModel>(this, "UserUpdated", sender => LoadData());
            MessagingCenter.Instance.Subscribe<BasePopUpViewModel>(this, "UserAnamnesisUpdated", sender => LoadData());

            _ = LoadData(true);
            _ = SyncHealthKitData();

            var toastService = DependencyService.Resolve<IToastService>();
        }

        private async Task SyncHealthKitData()
        {
            try
            {
                var now = DateTime.Now;
                var lastExportDateString = Preferences.Get("lastHealthDataExportDate", null);

                DateTime? lastHealthDataExportDate = null;
                if (lastExportDateString != null)
                {
                    lastHealthDataExportDate = DateTime.Parse(lastExportDateString);
                }

                if (healthKitService.IsEnabled() && lastHealthDataExportDate?.Date != now.Date)
                {
                    bool refresh = false;

                    var averageHeartRate = await healthKitService.FetchPreviousDayAverageHeartRate();
                    if (averageHeartRate != null)
                    {
                        var yesterday = DateTime.Now.AddDays(-1).Date;
                        var midYesterday = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 12, 0, 0);

                        var personalAnamnesis = new PersonalRecordViewModel
                        {
                            Date = midYesterday,
                            Value = averageHeartRate?.ToString("N1"),
                            Type = AnamnesisType.HeartRate,
                            IsPersonal_CreatorType = Device.RuntimePlatform == Device.Android ? PersonalAnamnesisCreatorType.GoogleFit : PersonalAnamnesisCreatorType.HealthKit
                        };

                        var response = await anamnesisService.SavePersonalAsync(personalAnamnesis);
                        if (response != null)
                        {
                            Preferences.Set("lastHealthDataExportDate", DateTime.Now.ToString());
                        }

                        refresh = true;
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

                        var response = await anamnesisService.SavePersonalAsync(personalAnamnesis);

                        if (response != null)
                        {
                            Preferences.Set("lastHealthDataExportDate", DateTime.Now.ToString());
                        }

                        refresh = true;
                    }

                    var systolicBloodPressure = await healthKitService.FetchPreviousDayAverageBloodPressureSystolic();
                    var diastolicBloodPressure = await healthKitService.FetchPreviousDayAverageBloodPressureDiastolic();

                    if (systolicBloodPressure != null && diastolicBloodPressure != null)
                    {
                        var yesterday = DateTime.Now.AddDays(-1).Date;
                        var midYesterday = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 12, 0, 0);

                        var personalAnamnesis = new PersonalRecordViewModel
                        {
                            Date = midYesterday,
                            Value = $"{systolicBloodPressure}/{diastolicBloodPressure}",
                            Type = AnamnesisType.BloodPressure
                        };

                        var response = await anamnesisService.SavePersonalAsync(personalAnamnesis);

                        if (response != null)
                        {
                            Preferences.Set("lastHealthDataExportDate", DateTime.Now.ToString());
                        }

                        refresh = true;
                    }

                    if (refresh)
                    {
                        await LoadData(false);
                    }
                }
            }
            catch (Exception ex)
            {
            }

        }

        private async Task GotoQuestionaire()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new QuestionnairePage());
        }

        private async Task GoToPhysicalActivity()
        {

            var vm = new PhysicalActivityPopUpViewModel(GeneralAnamnesis);
            await popupNavigation.PushAsync(new PhysicalActivityPopUp(vm));
        }

        private async Task GotoPersonalAnamnesis(AnamnesisType type)
        {
            try
            {
                var view = new PersonalAnamnesisDetailView
                {
                    BindingContext = new PersonalAnamnesisDetailViewModel(type)
                };

                await Application.Current.MainPage.Navigation.PushAsync(view);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private async Task GotoIsSmokerDetail()
        {
            var vm = new SmokerPopUpViewModel(abususAnamnesis)
            {
                Title = "Aktivní kuřák"
            };

            await popupNavigation.PushAsync(new SmokerPopUp(vm));
        }

        private async void GotoQuestionCreation()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new NewQuestionPage
            {
                BindingContext = new NewQuestionPageViewModel()
            });
        }
        private async Task LoadData(bool firstLoad = false)
        {
            try
            {

                // load user id before user detail
                if (firstLoad)
                {
                    IsBusy = true;

                    await userService.GetCurrentAsync();
                }

                var abususAnamnesisTask = anamnesisService.GetAbususAsync();
                var generalAnamnesisTask = anamnesisService.GetGeneralAsync();
                var userAnamnesisTask = anamnesisService.GetPersonalAsync();

                await Task.WhenAll(abususAnamnesisTask, generalAnamnesisTask, userAnamnesisTask);

                AbususAnamnesis = abususAnamnesisTask.Result;
                GeneralAnamnesis = generalAnamnesisTask.Result;
                var personalAnamnesis = userAnamnesisTask.Result;

                if (personalAnamnesis != null)
                {
                    var orderedAnamnesis = personalAnamnesis.OrderByDescending(a => a.IsPersonal_Date);
                    WeightAnamnesis = orderedAnamnesis.FirstOrDefault(a => a.IsPersonal_Type == (int)AnamnesisType.Weight);
                    BloodPressureAnamnesis = orderedAnamnesis.FirstOrDefault(a => a.IsPersonal_Type == (int)AnamnesisType.BloodPressure);
                    CholesterolAnamnesis = orderedAnamnesis.FirstOrDefault(a => a.IsPersonal_Type == (int)AnamnesisType.Cholesterol);
                    HeartRateAnamnesis = orderedAnamnesis.FirstOrDefault(a => a.IsPersonal_Type == (int)AnamnesisType.HeartRate);
                }

                if (firstLoad)
                {
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {

            }


        }

        private async Task WebLogIn()
        {
            IsBusy = true;

            var scanner = new MobileBarcodeScanner();

            var result = await scanner.Scan();

            if (result != null)
            {
                try
                {
                    var phoneAuth = JsonConvert.DeserializeObject<PhoneAuthResponseVM>(result.Text);
                    var dto = new PhoneAuthResponseDTO
                    {
                        AuthId = phoneAuth.Id,
                        Secret = phoneAuth.Secret
                    };

                    await userService.AuthorizeByPhone(dto);
                }
                catch (Exception ex)
                {
                    toastService.ShowError("Nepadařilo se zpracovat qr kód");
                }
            }

            IsBusy = false;
        }
    }
}

