using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.ViewModels.Questionnaire
{
    public class QuestionaireSymtomQuestionViewModel : BaseViewModel
    {
        private bool approved;

        public SymptomQuestionsDTO SymptomQuestion { get; set; }
        public bool Approved
        {
            get => approved;
            set => SetProperty(ref approved, value);
        }
        public QuestionaireSymtomQuestionViewModel(SymptomQuestionsDTO symptomQuestion)
        {
            SymptomQuestion = symptomQuestion;
        }
    }
}
