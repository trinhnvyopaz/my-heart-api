using DevExpress.XamarinForms.Charts;
using MyHeart.DTO.User;
using MyHeart.MobileApp.Enums;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.ViewModels.PopUps;
using MyHeart.MobileApp.Views.PopUp;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public class PersonalAnamnesisDetailViewModel : BaseViewModel
    {
        private UserAnamnesisViewModel userAnamnesis;
        private ObservableCollection<UserAnamnesisDTO> items;
        private AnamnesisService anamnesisService;

        private decimal? chartMaxY;
        private decimal? chartMaxX;
        private string valueSuffix;

        private DateTime dateTimeAxisMin;
        private DateTime dateTimeAxisMax;
        private DateTime dateTimeAxisVisualMin;
        private DateTime dateTimeAxisVisualMax;

        private DateTimeMeasureUnit currentChartTimeFrame;
        private AnamnesisType anamnesisType;

        IPopupNavigation popupNavigation = PopupNavigation.Instance;

        private ObservableCollection<ChartValue> firstLineValues = new ObservableCollection<ChartValue>();
        private ObservableCollection<ChartValue> secondLineValues = new ObservableCollection<ChartValue>();

        public UserAnamnesisViewModel UserAnamnesis
        {
            get => userAnamnesis;
            set => SetProperty(ref userAnamnesis, value);
        }

        public ObservableCollection<UserAnamnesisDTO> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        public decimal? ChartMaxY
        {
            get => chartMaxY;
            set => SetProperty(ref chartMaxY, value);
        }

        public decimal? ChartMinY
        {
            get => chartMaxX;
            set => SetProperty(ref chartMaxX, value);
        }

        public string ValueSuffix
        {
            get => valueSuffix;
            set => SetProperty(ref valueSuffix, value);
        }

        public ICommand GoBackCommand { get; set; }

        public DateTimeMeasureUnit CurrentChartTimeFrame
        {
            get => currentChartTimeFrame;
            set
            {
                IsBusy = true;
                SetAxisXRanges(value);
                SetProperty(ref currentChartTimeFrame, value);
                IsBusy = false;
            }
        }

        public ICommand EditItemCommand { get; set; }
        public ICommand CreateItemCommand { get; set; }
        public ICommand ChangeMeasureUnitCommand { get; set; }


        public ObservableCollection<ChartValue> FirstLineValues
        {
            get => firstLineValues;
            set => SetProperty(ref firstLineValues, value);
        }

        public ObservableCollection<ChartValue> SecondLineValues
        {
            get => secondLineValues;
            set => SetProperty(ref secondLineValues, value);
        }

        public DateTime DateTimeAxisMin
        {
            get => dateTimeAxisMin;
            set => SetProperty(ref dateTimeAxisMin, value);
        }

        public DateTime DateTimeAxisMax
        {
            get => dateTimeAxisMax;
            set => SetProperty(ref dateTimeAxisMax, value);
        }

        public DateTime DateTimeAxisVisualMin
        {
            get => dateTimeAxisVisualMin;
            set => SetProperty(ref dateTimeAxisVisualMin, value);
        }

        public DateTime DateTimeAxisVisualMax
        {
            get => dateTimeAxisVisualMax;
            set => SetProperty(ref dateTimeAxisVisualMax, value);
        }

        public PersonalAnamnesisDetailViewModel(AnamnesisType personalAnamnesisType)
        {
            anamnesisService = DependencyService.Resolve<AnamnesisService>();

            CurrentChartTimeFrame = DateTimeMeasureUnit.Day;
            anamnesisType = personalAnamnesisType;

            switch (personalAnamnesisType)
            {
                case AnamnesisType.BloodPressure:
                    Title = "Tlak krve";
                    break;
                case AnamnesisType.Cholesterol:
                    Title = "Hladina sérového LDL";
                    ValueSuffix = "mmol/l";
                    break;
                case AnamnesisType.Weight:
                    Title = "Hmotnost";
                    ValueSuffix = "Kg";
                    break;
                case AnamnesisType.HeartRate:
                    Title = "Tepová frekvence";
                    break;
            }

            EditItemCommand = new Command<UserAnamnesisDTO>(GoToEditItem);
            CreateItemCommand = new Command(GoToCreateItem);
            GoBackCommand = new Command(GoBack);
            ChangeMeasureUnitCommand = new Command<DateTimeMeasureUnit>(ChangeMeasureUnit);

            MessagingCenter.Subscribe<PersonalAnamnesisPopUpViewModel, int>(this, "PersonalAnamnesisDelete", (sender, id) => DeleteItem(id));
            MessagingCenter.Subscribe<PersonalAnamnesisPopUpViewModel, UserAnamnesisDTO>(this, "PersonalAnamnesisUpdate", (sender, anamnesis) => UpdateItem(anamnesis));
            MessagingCenter.Subscribe<PersonalAnamnesisPopUpViewModel, UserAnamnesisDTO>(this, "PersonalAnamnesisCreate", (sender, anamnesis) => CreateItem(anamnesis));

            _ = LoadData();
        }

        private void ChangeMeasureUnit(DateTimeMeasureUnit unit)
        {
            CurrentChartTimeFrame = unit;
        }

        private void CreateItem(UserAnamnesisDTO anamnesis)
        {
            Items.Add(anamnesis);
            InitChartData();
        }

        private void UpdateItem(UserAnamnesisDTO anamnesis)
        {
            var item = Items.FirstOrDefault(i => i.Id == anamnesis.Id);
            if (item != null)
            {
                var index = Items.IndexOf(item);
                Items[index] = anamnesis;
                InitChartData();
            }
        }

        private void DeleteItem(int id)
        {
            var item = Items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                Items.Remove(item);
                InitChartData();
            }
        }

        private async Task LoadData()
        {
            IsBusy = true;

            var personalAnamnesis = await anamnesisService.GetPersonalByTypeAsync(anamnesisType);
            if (personalAnamnesis != null)
            {
                Items = new ObservableCollection<UserAnamnesisDTO>(personalAnamnesis);
                InitChartData();
            }

            IsBusy = false;
        }

        private void InitChartData()
        {
            try
            {
                switch (anamnesisType)
                {
                    case AnamnesisType.Cholesterol:
                    case AnamnesisType.Weight:
                    case AnamnesisType.HeartRate:
                        var parsedItems = Items.Select(i => new ChartValue(ParseChartValue(i.IsPersonal_Value), i.IsPersonal_Date))
                             .Where(i => i.Value != null);

                        FirstLineValues = new ObservableCollection<ChartValue>(parsedItems);
                        break;
                    case AnamnesisType.BloodPressure:
                        var parsedFirstLineValues = Items.Select(i => new ChartValue(ParseChartValue(i.IsPersonal_Value, 0), i.IsPersonal_Date))
                            .Where(i => i.Value != null);

                        var parsedSecondLineValues = Items.Select(i => new ChartValue(ParseChartValue(i.IsPersonal_Value, 1), i.IsPersonal_Date))
                           .Where(i => i.Value != null);

                        FirstLineValues = new ObservableCollection<ChartValue>(parsedFirstLineValues);
                        SecondLineValues = new ObservableCollection<ChartValue>(parsedSecondLineValues);

                        if(firstLineValues.Count == 0)
                        {
                            FirstLineValues.Add(new ChartValue(0, new DateTime()));
                        }

                        break;
                }
   

            }
            catch (Exception ex)
            {
                toastService.ShowError("Nepodařilo se načíst data");
            }
        }

        private void SetAxisXRanges(DateTimeMeasureUnit measureUnit)
        {
            var now = DateTime.Now.Date;

            switch (measureUnit)
            {
                case DateTimeMeasureUnit.Week:

                    DateTimeAxisMin = now.AddDays(-20 * 7);
                    DateTimeAxisMax = now;

                    DateTimeAxisVisualMin = now.AddDays(-7 * 7);
                    DateTimeAxisVisualMax = now;
                    break;
                case DateTimeMeasureUnit.Month:
                    DateTimeAxisMin = now.AddMonths(-12);
                    DateTimeAxisMax = now;

                    DateTimeAxisVisualMin = now.AddMonths(-6);
                    DateTimeAxisVisualMax = now;
                    break;
                case DateTimeMeasureUnit.Year:
                    DateTimeAxisMin = now.AddYears(-10);
                    DateTimeAxisMax = now;

                    DateTimeAxisVisualMin = now.AddYears(-5);
                    DateTimeAxisVisualMax = now;
                    break;
                default:
                    DateTimeAxisMin = now.AddDays(-90);
                    DateTimeAxisMax = now;

                    DateTimeAxisVisualMin = now.AddDays(-7);
                    DateTimeAxisVisualMax = now;
                    break;
            }
        }

        private double? ParseChartValue(string value, int index)
        {
            try
            {
                var splitedValue = value.Split('/')[index];
                return double.Parse(splitedValue.Replace(',', '.'));
            }
            catch (Exception)
            {
                return null;
            }
        }
        private double? ParseChartValue(string value)
        {
            try
            {
                return double.Parse(value.Replace(',', '.'));
            }
            catch (Exception)
            {

                return null;
            }

        }

        private async void GoBack()
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }

        private async void GoToCreateItem()
        {
            var vm = new PersonalAnamnesisPopUpViewModel(null, anamnesisType);
            await popupNavigation.PushAsync(new PersonalAnamnesisPopUp(vm));
        }

        private async void GoToEditItem(UserAnamnesisDTO userAnamnesis)
        {
            var vm = new PersonalAnamnesisPopUpViewModel(userAnamnesis, anamnesisType);
            await popupNavigation.PushAsync(new PersonalAnamnesisPopUp(vm));
        }
    }

    public class ChartValue
    {
        public double? Value { get; set; }
        public DateTime Date { get; set; }

        public ChartValue(double? value, DateTime date)
        {
            Value = value;
            Date = date;
        }
    }
}
