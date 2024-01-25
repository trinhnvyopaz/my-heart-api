using MyHeart.MobileApp.ViewModels.Questionnaire;
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
    public partial class PickSymptomView : BaseQuestionaireStepView
    {
        public PickSymptomView()
        {
            InitializeComponent();
        }

        public override Task<bool> Validate()
        {
            var symptoms = SymptomList.ItemsSource as ObservableCollection<QuestionaireSymptomViewModel>;

            if(symptoms == null)
            {
                return Task.FromResult(false);
            }

            bool atleastOneChecked = symptoms.Any(s => s.IsChecked);

            return Task.FromResult(atleastOneChecked);
        }
    }
}