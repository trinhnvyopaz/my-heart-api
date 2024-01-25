using MyHeart.DTO;
using MyHeart.DTO.Questions;
using MyHeart.DTO.User;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.ViewModels.MyQuestion;
using MyHeart.MobileApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public class MyQuestionsPageViewModel : BaseViewModel
    {
        private QuestionsService _questionsService;
        private UserService _userService;
        private IToastService _toastService;

        private QuestionListViewModel newQuestion;
        private ObservableCollection<QuestionListDTO> openQuestions;

        DataTableRequest request;
        private int openQuestionPageNumber = 1;
        private int closedQuestionPageNumber = 1;
        private int newQuestionTabIndex = 1;

        private ObservableCollection<QuestionListDTO> closedQuestions;
        private int selectedTabIndex;

        public ICommand RefreshCommand { get; }
        public ICommand LoadMoreOpenQuestionsCommand { get; }
        public ICommand LoadMoreClosedQuestionsCommand { get; }
        public ICommand GoToChatPageCommnad { get; }
        public ICommand GoToNewQuastionPageCommnad { get; }
        public ICommand DeleteCommnad { get; }

        public QuestionListViewModel NewQuestion
        {
            get => newQuestion;
            set => SetProperty(ref newQuestion, value);
        }

        public ObservableCollection<QuestionListDTO> OpenQuestions
        {
            get => openQuestions;
            set => SetProperty(ref openQuestions, value);
        }
        public ObservableCollection<QuestionListDTO> ClosedQuestions
        {
            get => closedQuestions;
            set => SetProperty(ref closedQuestions, value);
        }
        public MyQuestionsPageViewModel()
        {
            _questionsService = DependencyService.Resolve<QuestionsService>();
            _userService = DependencyService.Resolve<UserService>();

            _toastService = DependencyService.Get<IToastService>();
            NewQuestion = new QuestionListViewModel();
            OpenQuestions = new ObservableCollection<QuestionListDTO>();

            RefreshCommand = new Command(() => LoadData());
            LoadMoreOpenQuestionsCommand = new Command(() => LoadMoreOpenQuestions());
            LoadMoreClosedQuestionsCommand = new Command(() => LoadMoreClosedQuestions());
            GoToChatPageCommnad = new Command<QuestionListDTO>((question) => Application.Current.MainPage.Navigation.PushModalAsync(new ChatPage(question)));
            GoToNewQuastionPageCommnad = new Command(() => Application.Current.MainPage.Navigation.PushAsync(new NewQuestionPage()));
            DeleteCommnad = new Command<int>(DeleteQuestion);

            MessagingCenter.Instance.Subscribe<NewQuestionPageViewModel, QuestionListDTO>(this, "QuestionCreated", OnQuestionCreated);

            _ = LoadData();
        }

        private async void DeleteQuestion(int id)
        {
            var result = await _questionsService.DeleteQuestion(id);
            if (result)
            {
                var openedQuestion = OpenQuestions.FirstOrDefault(x => x.Id == id);
                if (openedQuestion != null)
                {
                    OpenQuestions.Remove(openedQuestion);
                }
                else
                {
                    var closedQuestion = ClosedQuestions.FirstOrDefault(x => x.Id == id);
                    if (closedQuestion != null)
                    {
                        ClosedQuestions.Remove(closedQuestion);
                    }
                }
            }
            else
            {
                toastService.ShowError("Nepodařilo se vymazat dotaz");
            }
        }

        private void OnQuestionCreated(NewQuestionPageViewModel sender, QuestionListDTO question)
        {
            OpenQuestions.Add(question);
        }

        private async Task LoadMoreClosedQuestions()
        {
            closedQuestionPageNumber = closedQuestionPageNumber + 1;
            request = new DataTableRequest
            {
                Page = closedQuestionPageNumber,
                PageSize = 5
            };

            var closedQuestionList = await _questionsService.GetClosedQuestion(request);

            if (closedQuestionList != null)
            {
                foreach (var item in closedQuestionList.Data)
                {
                    ClosedQuestions.Add(item);
                }
            }
        }

        private async Task LoadMoreOpenQuestions()
        {
            openQuestionPageNumber = openQuestionPageNumber + 1;
            request = new DataTableRequest
            {
                Page = openQuestionPageNumber,
                PageSize = 5
            };

            var openQuestionList = await _questionsService.GetOpenQuestion(request);

            if (openQuestionList != null)
            {
                foreach (var item in openQuestionList.Data)
                {
                    OpenQuestions.Add(item);
                }
            }
        }

        private async Task LoadData()
        {
            IsBusy = true;

            openQuestionPageNumber = closedQuestionPageNumber = 1;

            request = new DataTableRequest
            {
                Page = openQuestionPageNumber,
                PageSize = 5
            };

            var openQuestionTask = _questionsService.GetOpenQuestion(request);
            var closedQuestionTask = _questionsService.GetClosedQuestion(request);

            await Task.WhenAll(openQuestionTask, closedQuestionTask);

            if (openQuestionTask.Result != null)
            {
                OpenQuestions = new ObservableCollection<QuestionListDTO>(openQuestionTask.Result.Data);
            }
            if (closedQuestionTask.Result != null)
            {
                ClosedQuestions = new ObservableCollection<QuestionListDTO>(closedQuestionTask.Result.Data);
            }

            IsBusy = false;
        }
    }
}
