using MyHeart.MobileApp.Views.MyAnamnesis;
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
    public partial class IconButtonView : ContentView
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(IconButtonView));
        public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(ImageSource), typeof(IconButtonView));
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(IconButtonView));



        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public IconButtonView()
        {
            InitializeComponent();

            MainGridTapGesture.SetBinding(TapGestureRecognizer.CommandProperty, new Binding(nameof(Command), source: this));
            IconImage.SetBinding(Image.SourceProperty, new Binding(nameof(Icon), source: this));
            TitleLabel.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));

        }
    }
}