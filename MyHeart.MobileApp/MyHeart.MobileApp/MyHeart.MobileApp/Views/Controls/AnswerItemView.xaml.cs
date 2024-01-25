using MyHeart.MobileApp.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnswerItemView : ContentView
    {
        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(AnswerItemView), propertyChanged: onCheckedChanged, defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty EnableTwoWayModeProperty = BindableProperty.Create(nameof(EnableTwoWayMode), typeof(bool), typeof(AnswerItemView), defaultValue: true);
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(AnswerItemView));

        private static void onCheckedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var answerItemView = (AnswerItemView)bindable;

            var isChecked = (bool)newValue;

            answerItemView.CheckedImage.IsVisible = isChecked;
            answerItemView.UncheckdFrame.IsVisible = !isChecked;
        }

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public bool EnableTwoWayMode
        {
            get => (bool)GetValue(EnableTwoWayModeProperty);
            set => SetValue(EnableTwoWayModeProperty, value);
        }

        public AnswerItemView()
        {
            InitializeComponent();

            TitleLabel.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));

            var boolToColorConverter = new BoolToColorConverter
            {
                TrueColor = (Color)App.Current.Resources["SecondaryColor"],
                FalseColor = Color.White
            };

            MainLayout.SetBinding(Frame.BackgroundColorProperty, new Binding(nameof(IsChecked), converter: boolToColorConverter, source: this));
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if(EnableTwoWayMode)
            {
                IsChecked = !IsChecked;
            }
           
        }
    }
}