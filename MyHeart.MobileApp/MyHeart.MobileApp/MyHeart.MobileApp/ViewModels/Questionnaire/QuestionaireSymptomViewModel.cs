using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.ViewModels.Questionnaire
{
    public class QuestionaireSymptomViewModel : BaseViewModel
    {
        private bool isChecked;
        private SymptomDTO symptom;

        public SymptomDTO Symptom
        { 
            get => symptom; 
            set => SetProperty(ref symptom, value);
        }
        public bool IsChecked
        {
            get => isChecked;
            set => SetProperty(ref isChecked, value);
        }

        public QuestionaireSymptomViewModel(SymptomDTO symptom)
        {
            Symptom = symptom;
        }
    }
}
