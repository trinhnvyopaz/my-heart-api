using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO.ProfessionInformation;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.Views.Questionnaire;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels.Questionnaire
{
    public class QuestionnairePageViewModel : BaseViewModel
    {
        private SymptomService symptomService;
        private SymtomQuestionService symptomQuestionService;
        private QuestionaireService questionaireService;
        private ObservableCollection<QuestionaireSymptomViewModel> symptoms;
        private string symptomQuery;

        public ICommand GoBackCommand { get; }
        public ICommand GoNextCommand { get; }
        public ICommand FindSymptomsCommand { get; }
        public ICommand ProcessQuestionCommad { get; }
        public ICommand LoadMoreCommand { get; set; }

        private DataTableRequest symptomsDataTableRequest;
        private int defaultPageLength = 50;
        private BaseQuestionaireStepView currentStep;
        private ObservableCollection<QuestionaireSymtomQuestionViewModel> questions;
        private QuestionaireSymtomQuestionViewModel currentQuestion;
        private int currentStepPosition;
        private int currentQuestionPosition;

        private string headerTitle;
        private bool FullyLoaded;
        private bool loadMoreEnabled = true;
        private Client_QuestionaireResultDTO questionairedResult;

        public Client_QuestionaireResultDTO QuestionairedResult
        {
            get => questionairedResult;
            set => SetProperty(ref questionairedResult, value);
        }
        public bool IsLastStep
        {
            get => CurrentStepPosition == Steps.Count - 1;
        }
        public int CurrentStepPosition
        {
            get => currentStepPosition;
            set => SetProperty(ref currentStepPosition, value);
        }
        public List<QuestionaireSymptomViewModel> SelectedSymtoms
        {
            get => Symptoms.Where(s => s.IsChecked).ToList();
        }
        public QuestionaireSymtomQuestionViewModel CurrentQuestion
        {
            get => currentQuestion;
            set => SetProperty(ref currentQuestion, value);
        }
        public BaseQuestionaireStepView CurrentStep
        {
            get => currentStep;
            set => SetProperty(ref currentStep, value);
        }

        public IList<BaseQuestionaireStepView> Steps { get; set; }
        public int CurrentQuestionPosition
        {
            get => currentQuestionPosition;
            set
            {
                if (SetProperty(ref currentQuestionPosition, value))
                {
                    HeaderTitle = $"{currentQuestionPosition + 1}/{Questions.Count}";
                }
            }
        }
        public string HeaderTitle
        {
            get => headerTitle;
            set => SetProperty(ref headerTitle, value);
        }
        public string SymptomQuery
        {
            get => symptomQuery;
            set => SetProperty(ref symptomQuery, value);
        }

        public ObservableCollection<QuestionaireSymptomViewModel> Symptoms
        {
            get => symptoms;
            set => SetProperty(ref symptoms, value);
        }


        public ObservableCollection<QuestionaireSymtomQuestionViewModel> Questions
        {
            get => questions;
            set => SetProperty(ref questions, value);
        }

        public QuestionnairePageViewModel()
        {
            symptomService = DependencyService.Resolve<SymptomService>();
            symptomQuestionService = DependencyService.Resolve<SymtomQuestionService>();
            questionaireService = DependencyService.Resolve<QuestionaireService>();

            FindSymptomsCommand = new Command(() => GetSymptoms());
            GoNextCommand = new Command(() => GoNext());
            GoBackCommand = new Command(() => GoBack());

            LoadMoreCommand = new Command(() => LoadMore());
            ProcessQuestionCommad = new Command(() => ProcessQuestion()); ;

            InitializaceSteps();

            _ = GetSymptoms();
        }

        private async Task LoadMore()
        {
            if (!FullyLoaded)
            {
                if (loadMoreEnabled)
                {
                    loadMoreEnabled = false;

                    symptomsDataTableRequest.Page = symptomsDataTableRequest.Page + 1;

                    var symptoms = await symptomService.GetSymptomsTable(symptomsDataTableRequest);

                    if (symptoms != null)
                    {
                        foreach (var symptom in symptoms.Data)
                        {
                            Symptoms.Add(new QuestionaireSymptomViewModel(symptom));
                        }
                        SortSympmtoms();
                        FullyLoaded = symptoms.TotalCount == Symptoms.Count;
                    }

                    loadMoreEnabled = true;
                }


            }
        }
        private void SortSympmtoms()
        {
            var comparer = new CustomStringComparer(StringComparer.CurrentCulture, SymptomQuery);
            Symptoms = new ObservableCollection<QuestionaireSymptomViewModel>(Symptoms.OrderBy(x => x.Symptom.Name, comparer));
        }
        private async Task GoBack()
        {
            var previousPosition = CurrentStepPosition - 1;

            if (IsLastStep)
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
                return;
            }

            if (previousPosition >= 0)
            {
                CurrentStepPosition = previousPosition;
                HeaderTitle = "";
            }
            else
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
        }

        private void ProcessQuestion()
        {
            var nextIndex = questions.IndexOf(CurrentQuestion) + 1;
            if (nextIndex < questions.Count)
            {
                CurrentQuestion = questions[nextIndex];
            }
            else
            {
                GoNext();
            }
        }

        private async void GoNext()
        {
            var isValid = await CurrentStep.Validate();

            if (isValid)
            {
                CurrentStepPosition++;

                if (!IsLastStep)
                {
                    if (CurrentStep.GetType() == typeof(QuestionView))
                    {
                        _ = GetQuestions();
                    }
                }
                else
                {
                    HeaderTitle = "";
                    _ = CreateQuestionaire();
                }
                ReloadProperties();
            }
        }
        private async Task CreateQuestionaire()
        {
            var questionaire = new Client_QuestionaireDTO
            {
                QuestionAnswers = new List<Client_QuestionAnswerDTO>()
            };

            foreach (var question in Questions)
            {
                questionaire.QuestionAnswers.Add(new Client_QuestionAnswerDTO
                {
                    SymptomQuestionId = question.SymptomQuestion.Id,
                    Approved = question.Approved
                });
            }

            var createdQuestionaire = await questionaireService.CreateQuestionaire(questionaire);

            if(createdQuestionaire != null)
            {
                QuestionairedResult = await questionaireService.GetResult(createdQuestionaire.Id);
            }
            else
            {
                toastService.ShowError("Nepodařilo se vytvořit rozhovor");
            }
       
        }
        private void ReloadProperties()
        {
            OnPropertyChanged(nameof(IsLastStep));
        }
        public async Task GetQuestions()
        {
            IsBusy = true;

            var symptomIds = SelectedSymtoms.Select(q => q.Symptom.Id);

            var symptomQuestions = await symptomQuestionService.GetSymptomQuestionListBySymptomIds(symptomIds);

            if (symptomQuestions != null)
            {
                Questions = new ObservableCollection<QuestionaireSymtomQuestionViewModel>(symptomQuestions.Select(s => new QuestionaireSymtomQuestionViewModel(s)));
                HeaderTitle = $"1/{Questions.Count}";
            }

            IsBusy = false;
        }
        public async Task GetSymptoms()
        {
            IsBusy = true;

            symptomsDataTableRequest = new DataTableRequest
            {
                Page = 1,
                PageSize = defaultPageLength
            };

            var symptoms = await symptomService.GetSymptomsTable(symptomsDataTableRequest);

            if (symptoms != null)
            {
                Symptoms = new ObservableCollection<QuestionaireSymptomViewModel>(symptoms.Data.Select(s => new QuestionaireSymptomViewModel(s)));
                FullyLoaded = symptoms.TotalCount == Symptoms.Count;
                SortSympmtoms();
            }

            IsBusy = false;
        }

        private void InitializaceSteps()
        {
            Steps = new List<BaseQuestionaireStepView>
            {
                new PickSymptomView
                {
                    BindingContext = this
                },
                new QuestionView
                {
                    BindingContext = this
                },
                new QuestionaireResultView
                {
                    BindingContext = this
                }
            };
        }
    }

    public class CustomStringComparer : IComparer<string>
    {
        private readonly IComparer<string> _baseComparer;
        private string _text;
        public CustomStringComparer(IComparer<string> baseComparer, string text)
        {
            _baseComparer = baseComparer;
            _text = text;
        }
        public int Compare(string x, string y)
        {

            x = x.ToLower();
            y = y.ToLower();

            if(!string.IsNullOrEmpty(_text))
            {
                _text = _text.ToLower();
                if (x.Contains(_text) && !y.Contains(_text))
                {
                    return -1;
                }
                else if (y.Contains(_text) && !x.Contains(_text))
                {
                    return 1;
                }
            }

            return _baseComparer.Compare(x, y);
        }
    }
}
