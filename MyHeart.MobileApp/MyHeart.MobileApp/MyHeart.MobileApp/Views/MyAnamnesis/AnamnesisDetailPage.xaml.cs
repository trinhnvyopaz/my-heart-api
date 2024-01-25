using Rg.Plugins.Popup.Pages;
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
    public partial class AnamnesisDetailPage : PopupPage
    {
        public static BindableProperty ItemViewProperty = BindableProperty.Create(nameof(ItemView), typeof(View), typeof(AnamnesisDetailPage));
        public static BindableProperty SubtitleProperty = BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(AnamnesisDetailPage));
        public static BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(string), typeof(AnamnesisDetailPage));
        public static BindableProperty HeaderFontColorProperty = BindableProperty.Create(nameof(HeaderFontColor), typeof(Color), typeof(AnamnesisDetailPage), defaultValue: (Color)Application.Current.Resources["SecondaryColor"]);
        public View ItemView
        {
            get => (View)GetValue(ItemViewProperty);
            set => SetValue(ItemViewProperty, value);
        }
        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public Color HeaderFontColor
        {
            get => (Color)GetValue(HeaderFontColorProperty);
            set => SetValue(HeaderFontColorProperty, value);
        }
        public AnamnesisDetailPage()
        {
            InitializeComponent();
        }
    }
}