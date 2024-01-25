using DevExpress.XamarinForms.Editors;
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
    public partial class FormTimeInputView : ContentView
    {
        public static readonly BindableProperty TimeProperty = BindableProperty.Create(nameof(Time), typeof(TimeSpan), typeof(FormTimeInputView), defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(FormTimeInputView), defaultValue: Keyboard.Default);
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(FormTimeInputView));
        public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(FormTimeInputView), defaultValue: false);
        public static readonly BindableProperty InputColorProperty = BindableProperty.Create(nameof(InputColor), typeof(Color), typeof(FormTimeInputView), defaultValue: Color.White);
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(InputColor), typeof(string), typeof(FormTimeInputView));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }
        public Color InputColor
        {
            get => (Color)GetValue(InputColorProperty);
            set => SetValue(InputColorProperty, value);
        }
        public TimeSpan Time
        {
            get => (TimeSpan)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }
        public Keyboard Keyboard
        {
            get => (Keyboard)GetValue(KeyboardProperty);
            set => SetValue(KeyboardProperty, value);
        }
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public bool IsPassword
        {
            get => (bool)GetValue(IsPasswordProperty);
            set => SetValue(IsPasswordProperty, value);
        }

        public FormTimeInputView()
        {
            InitializeComponent();            

            InputTitle.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));
            TimePicker.SetBinding(TimeEdit.TimeSpanProperty, new Binding(nameof(Time), source: this));
            InputFrame.SetBinding(Frame.BackgroundColorProperty, new Binding(nameof(InputColor), source: this));
        }
 
    }
}