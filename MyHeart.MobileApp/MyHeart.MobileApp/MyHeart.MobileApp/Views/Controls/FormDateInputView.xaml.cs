﻿using DevExpress.XamarinForms.Editors;
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
    public partial class FormDateInputView : ContentView
    {
        public static readonly BindableProperty DateProperty = BindableProperty.Create(nameof(Date), typeof(DateTime), typeof(FormInputView), defaultBindingMode: BindingMode.TwoWay, defaultValue: DateTime.Now);
        public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(FormInputView), defaultValue: Keyboard.Default);
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(FormInputView));
        public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(FormInputView), defaultValue: false);
        public static readonly BindableProperty InputColorProperty = BindableProperty.Create(nameof(InputColor), typeof(Color), typeof(FormInputView), defaultValue: Color.White);
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(InputColor), typeof(string), typeof(FormInputView));
        public static readonly BindableProperty ShowIconProperty = BindableProperty.Create(nameof(ShowIcon), typeof(bool), typeof(FormInputView), defaultValue: true);

        public bool ShowIcon
        {
            get => (bool)GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }
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
        public DateTime Date
        {
            get => (DateTime)GetValue(DateProperty);
            set => SetValue(DateProperty, value);
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

        public FormDateInputView()
        {
            InitializeComponent();

            InputTitle.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));
            DatePicker.SetBinding(DateEdit.DateProperty, new Binding(nameof(Date), source: this));
            InputFrame.SetBinding(Frame.BackgroundColorProperty, new Binding(nameof(InputColor), source: this));
            DatePicker.SetBinding(DateEdit.IsDateIconVisibleProperty, new Binding(nameof(ShowIcon), source: this));
        }

    }
}