using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views.MyAnamnesis
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnamnesisCardView : ContentView
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(AnamnesisCardView));
        public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(ImageSource), typeof(AnamnesisCardView));

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

        public AnamnesisCardView()
        {
            InitializeComponent();
            TitleLabel.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));
            IconImage.SetBinding(Image.SourceProperty, new Binding(nameof(Icon), source: this));
        }
    }
}