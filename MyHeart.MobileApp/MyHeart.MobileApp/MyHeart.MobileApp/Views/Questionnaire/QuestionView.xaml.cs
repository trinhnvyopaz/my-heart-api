using MyHeart.DTO.ProfessionalInformation;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.ViewModels.Questionnaire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views.Questionnaire
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionView : BaseQuestionaireStepView
    {
        private SymtomQuestionService symtomQuestionService;
        private IToastService toastService;

        public QuestionView()
        {
            InitializeComponent();
            toastService = DependencyService.Resolve<IToastService>();
        }

        private void QuestionCarousel_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            Header = $"{e.CurrentPosition}/{QuestionCarousel.ItemsSource.Cast<QuestionaireSymtomQuestionViewModel>().Count()}";
        }
        public async override Task<bool> Validate()
        {
            if (QuestionCarousel.Position == 0)
            {
                toastService.ShowError("Odpovězte alespoň na jednu otázku");
                return false;
            }

            var items = QuestionCarousel.ItemsSource.Cast<QuestionaireSymtomQuestionViewModel>();

            if (QuestionCarousel.Position < items.Count() - 1)
            {
                var result = await Application.Current.MainPage.DisplayAlert("", "Neodpověděli jste na všechny otázky. Chcete opravdu chcete pokračovat ?", "Ano", "Ne");              
                return result;
            }

            return true;
        }
    }
}