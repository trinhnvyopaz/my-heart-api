using MyHeart.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels.MyQuestion
{
    public class NewQuestionPageViewModel : BaseViewModel
    {
        private string _subject;
        private string _message;

        public ICommand GoBackCommand { get; set; }
        public ICommand SendCommand { get; set; }

        private QuestionsService _questionsService;
        private IToastService _toastService;

        public string Subject
        {
            get => _subject;
            set => SetProperty(ref _subject, value);
        }
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }
        public NewQuestionPageViewModel()
        {
            GoBackCommand = new Command(() => App.Current.MainPage.Navigation.PopAsync());
            SendCommand = new Command(() => SendQuestion());

            _questionsService = DependencyService.Resolve<QuestionsService>();
            _toastService = DependencyService.Resolve<IToastService>();
        }

        private string ParseStringToHtml(string body)
        {
            return body.Replace("\n", "<br>");
        }

        private async Task SendQuestion()
        {
            var questionDuplicate = new QuestionListViewModel
            {
                Id = -1,
                UserId = -1,
                Subject = _subject,
                Body = ParseStringToHtml(_message)
            };

            var question = await _questionsService.AskQuestion(questionDuplicate);

            if (question != null)
            {
                _toastService.ShowSuccess("Uloženo");
                MessagingCenter.Instance.Send(this, "QuestionCreated", question);
                await App.Current.MainPage.Navigation.PopAsync();
            }

            else
            {
                _toastService.ShowError("Nepodařilo se uložit položku");
            }
        }
    }
}
