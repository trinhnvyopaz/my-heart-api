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
    public partial class SearchBarView : ContentView
    {
        public static readonly BindableProperty SearchQueryProperty = BindableProperty.Create(nameof(SearchQuery), typeof(string), typeof(SearchBarView));

        public string SearchQuery
        {
            get => (string)GetValue(SearchQueryProperty);
            set => SetValue(SearchQueryProperty, value);
        }
        public SearchBarView()
        {
            InitializeComponent();
            SearchEntry.SetBinding(Entry.TextProperty, new Binding(nameof(SearchQuery), source: this));
        }
    }
}