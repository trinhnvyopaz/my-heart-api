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
    public partial class QuestionaireResultView : BaseQuestionaireStepView
    {
        public QuestionaireResultView()
        {
            InitializeComponent();
        }
    }
}