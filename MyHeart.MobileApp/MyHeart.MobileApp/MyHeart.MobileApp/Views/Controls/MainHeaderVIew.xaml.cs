using MyHeart.MobileApp.Services;
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
    public partial class MainHeaderView : ContentView
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(MainHeaderView));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string UserName { get; set; }

        public MainHeaderView()
        {
            InitializeComponent();
            TitleLabel.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));

            var userService = DependencyService.Resolve<UserService>();        
            UserName = $"{userService.User.FirstName} {userService.User.LastName}";
            UserLabel.SetBinding(Label.TextProperty, new Binding(nameof(UserName), source: this));
        }
    }
}