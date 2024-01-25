using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Views.Questionnaire
{
    public abstract class BaseQuestionaireStepView : ContentView
    {
        public static BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header),typeof(string), typeof(BaseQuestionaireStepView),defaultBindingMode: BindingMode.TwoWay, defaultValue: "Mám obtíže");

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public virtual Task<bool> Validate()
        {
            return Task.FromResult(true);
        }
    }
}
