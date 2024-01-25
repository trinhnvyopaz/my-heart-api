using MyHeart.MobileApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NonPharmacyCollectionView : ContentView
    {
        public NonPharmacyCollectionView()
        {
            InitializeComponent();
        }

        private void NonPharmacyDetailView_EntryFocused(object sender, FocusEventArgs e)
        {
            if(DeviceInfo.Platform == DevicePlatform.iOS)
            {
                var dataSource = (ObservableCollection<UserNonpharmacyViewModel>)ItemCollectionView.ItemsSource;
                var index = dataSource.IndexOf(sender as UserNonpharmacyViewModel);

                ItemCollectionView.ScrollTo(index, position: ScrollToPosition.Start, animate: false);
            }    
        }
    }
}