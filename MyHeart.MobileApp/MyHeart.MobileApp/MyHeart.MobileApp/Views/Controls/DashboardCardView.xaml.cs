using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardCardView : ContentView
    {
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(string), typeof(DashboardCardView));
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(DashboardCardView));
        public static readonly BindableProperty SubtitleProperty = BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(DashboardCardView));
        public static readonly BindableProperty ProgressProperty = BindableProperty.Create(nameof(Progress), typeof(string), typeof(DashboardCardView));
        public static readonly BindableProperty UnitProperty = BindableProperty.Create(nameof(Unit), typeof(string), typeof(DashboardCardView));
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(DashboardCardView));
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(DashboardCardView));
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(DashboardCardView), defaultValue: Application.Current.Resources["MainColor"]);
        public static readonly BindableProperty ValueColorProperty = BindableProperty.Create(nameof(ValueColor), typeof(Color), typeof(DashboardCardView), defaultValue: Application.Current.Resources["MainColor"]);
        public static readonly BindableProperty IsBigProperty = BindableProperty.Create(nameof(IsBig), typeof(bool), typeof(DashboardCardView), defaultValue: true);

        public bool IsBig
        {
            get => (bool)GetValue(IsBigProperty);
            set => SetValue(IsBigProperty, value);
        }
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public string Progress
        {
            get => (string)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }
        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }
        public string Unit
        {
            get => (string)GetValue(UnitProperty);
            set => SetValue(UnitProperty, value);
        }
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        public Color ValueColor
        {
            get => (Color)GetValue(ValueColorProperty);
            set => SetValue(ValueColorProperty, value);
        }
        public DashboardCardView()
        {
            InitializeComponent();
            FrameTapGesture.SetBinding(TapGestureRecognizer.CommandProperty, new Binding(nameof(Command), source: this));
            FrameTapGesture.SetBinding(TapGestureRecognizer.CommandParameterProperty, new Binding(nameof(CommandParameter), source: this));

            MainFrame.SetBinding(BackgroundColorProperty, new Binding(nameof(Color), source: this));
            TitleLabel.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));
            ValueLabel.SetBinding(Label.TextProperty, new Binding(nameof(Value), source: this));
            ValueLabel.SetBinding(Label.TextColorProperty, new Binding(nameof(ValueColor), source: this));
            SubtitleLabel.SetBinding(Label.TextProperty, new Binding(nameof(Subtitle), source: this));
            SubtitleLabel.SetBinding(Label.IsVisibleProperty, new Binding(nameof(IsBig), source: this));
            UnitLabel.SetBinding(Label.TextColorProperty, new Binding(nameof(ValueColor), source: this));
            UnitLabel.SetBinding(Label.TextProperty, new Binding(nameof(Unit), source: this));
            UnitProgressLabel.SetBinding(Label.TextProperty, new Binding(nameof(Unit), source: this));
            UnitProgressLabel.SetBinding(Label.IsVisibleProperty, new Binding(nameof(IsBig), source: this));
            ProgressLabel.SetBinding(Label.TextProperty, new Binding(nameof(Progress), source: this));
        }
    }
}