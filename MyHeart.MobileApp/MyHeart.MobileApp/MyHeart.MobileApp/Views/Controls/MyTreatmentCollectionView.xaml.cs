using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.Views.MyAnamnesis;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyTreatmentCollectionView : ContentView
    {
        private View view;
        private bool isEntryFocused;

        public MyTreatmentCollectionView()
        {
            InitializeComponent();

            var keyboardService = DependencyService.Resolve<IKeyboardService>();
            if (keyboardService != null && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                keyboardService.OnKeyBoardSizeChanged += KeyboardService_OnKeyBoardSizeChanged;
            }
        }


        private void KeyboardService_OnKeyBoardSizeChanged(object sender, KeyBoardSizeChangedEventArgs e)
        {
            if (isEntryFocused)
            {
                double offset = 100;
                double screenHeight = Application.Current.MainPage.Height;
                       
                if (screenHeight > 850)
                {
                    offset = 300;
                }
                else if (screenHeight > 700)
                {
                    offset = 225;
                }
                else if (screenHeight > 500)
                {
                    offset = 175;
                }

                var viewCoords = view.Y + offset;

                bool isHidden = viewCoords + view.Height > screenHeight - e.Height;

                var parent = FindParent(view);

                var page = GetAnamnesisAddPage(view);
                parent.TranslationY = 0;
                if (isHidden)
                {
                    parent.TranslationY = (screenHeight - e.Height) - (viewCoords + 2 *view.Height);
                }
            }
        }

        private void MyTreatmentDetailView_EntryFocused(object sender, FocusEventArgs e)
        {

            view = sender as View;
            isEntryFocused = e.IsFocused;
        }

        private View FindParent(View view)
        {
            if (view == null)
                return null;

            while (view.Parent.GetType() != typeof(CollectionView))
            {

                view = (View)view.Parent;

                if (view == null)
                    return null;
            }

            return view as View;
        }
        private AnamnesisAddPage GetAnamnesisAddPage(View view)
        {
            if (view == null)
                return null;

            while (view.Parent != null)
            {

                if (view.Parent is AnamnesisAddPage page)
                    return page;


                view = (View)view.Parent;
            }

            return null;
        }
    }
}