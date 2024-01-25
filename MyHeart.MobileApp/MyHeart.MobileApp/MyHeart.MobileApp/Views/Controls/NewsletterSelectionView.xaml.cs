using Plugin.InputKit.Shared.Controls;
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
    public partial class NewsletterSelectionView : ContentView
    {
        public static BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(NewsletterSelectionView), defaultBindingMode: BindingMode.TwoWay);
        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }
        public NewsletterSelectionView()
        {
            InitializeComponent();
            RadionButtonView.SetBinding(RadioButtonGroupView.SelectedIndexProperty, new Binding(nameof(SelectedIndex), BindingMode.TwoWay, source: this));
        }
    }
}