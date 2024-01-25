using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views.MyAnamnesis
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyTreatmentListPage : ContentPage
    {
        Timer timer;
        public MyTreatmentListPage()
        {
            InitializeComponent();
            timer = new Timer(250);
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                timer.Stop();
                AddItemButton.IsVisible = true;
            });       
        }

        private void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if (timer.Enabled == false)
            {
                timer.Enabled = true;
                timer.Start();
                AddItemButton.IsVisible = false;
            }
            else
            {
                timer.Stop();
                timer.Start();
            }
        }
    }
}