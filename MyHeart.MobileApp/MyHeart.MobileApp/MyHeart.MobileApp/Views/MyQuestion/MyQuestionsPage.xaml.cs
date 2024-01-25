using MyHeart.MobileApp.ViewModels;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyQuestionsPage : ContentPage
    {
        private int newQuestionTabIndex = 1;
        bool swipeIsOpen;

        public MyQuestionsPage()
        {

            BindingContext = new MyQuestionsPageViewModel();
            InitializeComponent();
            UpdateTabHeaderColors();
            //TabView.PropertyChanged += TabView_PropertyChanged;
            //MessagingCenter.Instance.Subscribe<DashboardPageViewModel>(this, "OnGoToNewQuestion", sender =>
            //{
            //    TabView.SetPosition(newQuestionTabIndex);
            //});



        }

        private void TabView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedTabIndex")
            {
                UpdateTabHeaderColors();
            }
        }

        private void UpdateTabHeaderColors()
        {
            //var currentTab = TabView.ItemSource.FirstOrDefault(i => i.IsCurrent);
            //currentTab.HeaderTextColor = (Color)Application.Current.Resources["MainColor"];
            //foreach (var item in TabView.ItemSource.Where(i => !i.IsCurrent).ToList())
            //{
            //    item.HeaderTextColor = Color.Black;
            //}
        }
    }
}