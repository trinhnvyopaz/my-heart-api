using MyHeart.MobileApp.Services;
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
    public partial class AnamnesisAddPage : PopupPage
    {
        public static BindableProperty ItemsViewProperty = BindableProperty.Create(nameof(ItemsView), typeof(View), typeof(AnamnesisAddPage));
        public static BindableProperty SecondSearchVisisbleProperty = BindableProperty.Create(nameof(SecondSearchVisisble), typeof(bool), typeof(AnamnesisAddPage));
        public static BindableProperty SearchPlaceholderProperty = BindableProperty.Create(nameof(SearchPlaceholder), typeof(string), typeof(AnamnesisAddPage), defaultValue: "Vyhledat");
        public static BindableProperty SecondSearchPlaceholderProperty = BindableProperty.Create(nameof(SecondSearchPlaceholder), typeof(string), typeof(AnamnesisAddPage));

        public bool SecondSearchVisisble
        {
            get => (bool)GetValue(SecondSearchVisisbleProperty);
            set => SetValue(SecondSearchVisisbleProperty, value);
        }
        public string SearchPlaceholder
        {
            get => (string)GetValue(SearchPlaceholderProperty);
            set => SetValue(SearchPlaceholderProperty, value);
        }
        public string SecondSearchPlaceholder
        {
            get => (string)GetValue(SecondSearchPlaceholderProperty);
            set => SetValue(SecondSearchPlaceholderProperty, value);
        }
        public View ItemsView
        {
            get => (View)GetValue(ItemsViewProperty);
            set => SetValue(ItemsViewProperty, value);
        }

        public string Subtitle { get; set; }

        public AnamnesisAddPage(string subtitle)
        {
            InitializeComponent();
            Subtitle = subtitle;

            SubtitleLabel.SetBinding(Label.TextProperty, new Binding(nameof(Subtitle), source: this));
        }
    }
}