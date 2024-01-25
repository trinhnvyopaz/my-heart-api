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
    public partial class FormEditorInputView : ContentView
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(FormInputView), defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(FormInputView), defaultValue: Keyboard.Default);
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(FormInputView));
        public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(FormInputView), defaultValue: false);
        public static readonly BindableProperty InputColorProperty = BindableProperty.Create(nameof(InputColor), typeof(Color), typeof(FormInputView), defaultValue: Color.White);
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(InputColor), typeof(string), typeof(FormInputView));

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
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
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

        public FormEditorInputView()
        {
            InitializeComponent();

            InputTitle.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));
            FormInput.SetBinding(Entry.KeyboardProperty, new Binding(nameof(Keyboard), source: this));
            FormInput.SetBinding(Entry.TextProperty, new Binding(nameof(Text), source: this));
            FormInput.SetBinding(Entry.IsPasswordProperty, new Binding(nameof(IsPassword), source: this));
            FormInput.SetBinding(Entry.PlaceholderProperty, new Binding(nameof(Placeholder), source: this));
            InputFrame.SetBinding(Frame.BackgroundColorProperty, new Binding(nameof(InputColor), source: this));
        }
    }
}