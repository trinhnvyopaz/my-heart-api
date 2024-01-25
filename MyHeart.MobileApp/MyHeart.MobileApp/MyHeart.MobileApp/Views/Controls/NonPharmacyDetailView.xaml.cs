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
    public partial class NonPharmacyDetailView : ContentView
    {
        public event EventHandler<FocusEventArgs> EntryFocused;
        public NonPharmacyDetailView()
        {
            InitializeComponent();
        }

        private void FormInputView_EntryFocused(object sender, FocusEventArgs e)
        {
            EntryFocused?.Invoke(BindingContext, e);
        }
    }
}