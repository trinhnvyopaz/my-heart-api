using MyHeart.DTO.Client;
using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO.ProfessionInformation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views.Questionnaire
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiseasesResultView : ContentView
    {
        public static BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(DiseasesResultView));
        public static BindableProperty FrameColorProperty = BindableProperty.Create(nameof(FrameColor), typeof(Color), typeof(DiseasesResultView));
        public static BindableProperty ItemsProperty = BindableProperty.Create(nameof(Items), typeof(ICollection<Client_QuestionaireDiseaseAverageDTO>), typeof(DiseasesResultView));
        public static BindableProperty FrameBorderColorProperty = BindableProperty.Create(nameof(FrameBorderColor), typeof(Color), typeof(DiseasesResultView));
        public static BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(DiseasesResultView));

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public Color FrameColor
        {
            get => (Color)GetValue(FrameColorProperty);
            set => SetValue(FrameColorProperty, value);
        }

        public Color FrameBorderColor
        {
            get => (Color)GetValue(FrameBorderColorProperty);
            set => SetValue(FrameBorderColorProperty, value);
        }

        public ICollection<Client_QuestionaireDiseaseAverageDTO> Items
        {
            get => (ICollection<Client_QuestionaireDiseaseAverageDTO>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public DiseasesResultView()
        {
            InitializeComponent();

            TitleLabel.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));
            MainFrame.SetBinding(Frame.BackgroundColorProperty, new Binding(nameof(FrameColor), source: this));
            MainFrame.SetBinding(Frame.BorderColorProperty, new Binding(nameof(FrameBorderColor), source: this));
        }
    }
}