using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyTreatmentDetailView : ContentView
    {
        public static BindableProperty ShowDosageProperty = BindableProperty.Create(nameof(ShowDosage), typeof(bool), typeof(MyTreatmentDetailView));
        public static BindableProperty ShowDosageTitleAndValueProperty = BindableProperty.Create(nameof(ShowDosageTitleAndValue), typeof(bool), typeof(MyTreatmentDetailView));
        public event EventHandler<FocusEventArgs> EntryFocused;

        bool isEntryFocused;
        FormInputView view;
        bool shiftedUp;
            
        public bool ShowDosage
        {
            get => (bool)GetValue(ShowDosageProperty);
            set => SetValue(ShowDosageProperty, value);
        }
        public bool ShowDosageTitleAndValue
        {
            get => (bool)GetValue(ShowDosageTitleAndValueProperty);
            set => SetValue(ShowDosageTitleAndValueProperty, value);
        }
        public MyTreatmentDetailView()
        {
            InitializeComponent();
            DosageTitleLabel.SetBinding(IsVisibleProperty, new Binding(nameof(ShowDosageTitleAndValue), mode: BindingMode.TwoWay, source: this));
            DosageValueLabel.SetBinding(IsVisibleProperty, new Binding(nameof(ShowDosageTitleAndValue), mode: BindingMode.TwoWay, source: this));
        }

        private void FormInputView_FocusedChanged(object sender, FocusEventArgs e)
        {
            EntryFocused?.Invoke(sender, e);
        }
    }
}