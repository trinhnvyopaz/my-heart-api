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
    public partial class TitleHeader : ContentView
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(TitleHeader));
        public static readonly BindableProperty ActionCommandProperty = BindableProperty.Create(nameof(ActionCommand), typeof(ICommand), typeof(TitleHeader));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public ICommand ActionCommand
        {
            get => (ICommand)GetValue(ActionCommandProperty);
            set => SetValue(ActionCommandProperty, value);
        }

        public TitleHeader()
        {
            InitializeComponent();
            TitleLabel.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));
            BackButton.SetBinding(Button.CommandProperty, new Binding(nameof(ActionCommand), source: this));
        }
    }
}