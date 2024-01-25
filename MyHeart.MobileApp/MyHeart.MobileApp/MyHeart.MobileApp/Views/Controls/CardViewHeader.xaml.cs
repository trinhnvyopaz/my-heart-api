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
    public partial class CardViewHeader : ContentView
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(CardViewHeader));
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(string), typeof(CardViewHeader));
        public static readonly BindableProperty ActionCommandProperty = BindableProperty.Create(nameof(Value), typeof(ICommand), typeof(CardViewHeader));
        public static readonly BindableProperty ActionCommandParameterProperty = BindableProperty.Create(nameof(ActionCommandParameter), typeof(object), typeof(CardViewHeader));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public ICommand ActionCommand
        {
            get => (ICommand)GetValue(ActionCommandProperty);
            set => SetValue(ActionCommandProperty, value);
        }
        public object ActionCommandParameter
        {
            get => (object)GetValue(ActionCommandParameterProperty);
            set => SetValue(ActionCommandParameterProperty, value);
        }
        public CardViewHeader()
        {
            InitializeComponent();

            TitleLabel.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));
            ValueLabel.SetBinding(Label.TextProperty, new Binding(nameof(Value), source: this));
        }
    }
}