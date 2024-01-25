using MyHeart.DTO;
using MyHeart.DTO.Questions;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MyHeart.MobileApp.ViewModels
{
    public class ChatPageViewModel : BaseViewModel
    {
        private ObservableCollection<QuestionCommentViewModel> messages;
        private QuestionCommentViewModel newMessage;
        private QuestionListDTO question;

        private int defaultPageSize = 10;
        private int currentPageSize;
        private QuestionCommentRequest questionRequest;

        private bool oldRepliesLoaded = false;
        private bool isMessageSending;

        ChatService chatService;
        UserService userService;
        QuestionsService questionsService;
        IToastService toastService;

        public ObservableCollection<QuestionCommentViewModel> Messages
        {
            get => messages;
            set => SetProperty(ref messages, value);
        }
        public string QuestionSubject { get; }
        public ICommand SendCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand StartVideoChatCommand { get; }

        public bool IsMessageSending
        {
            get => isMessageSending;
            set => SetProperty(ref isMessageSending, value);
        }

        public QuestionCommentViewModel NewMessage
        {
            get => newMessage;
            set => SetProperty(ref newMessage, value);
        }
        public QuestionListDTO Question
        {
            get => question;
            set => SetProperty(ref question, value);
        }

        public ChatPageViewModel(QuestionListDTO question)
        {
            Messages = new ObservableCollection<QuestionCommentViewModel>();
            CreateNewMessage();
            QuestionSubject = question.Subject;

            this.question = question;

            SendCommand = new Command(() => SendMessage());
            GoBackCommand = new Command(GoBack);
            StartVideoChatCommand = new Command(() => GoToVideo());


            questionsService = DependencyService.Resolve<QuestionsService>();
            userService = DependencyService.Resolve<UserService>();
            toastService = DependencyService.Get<IToastService>();

            Application.Current.ModalPopping += Current_ModalPopping;

            questionRequest = new QuestionCommentRequest();

            _ = LoadData();
        }

        private async Task GoToVideo()
        {
            if (Device.RuntimePlatform == Device.iOS && DeviceInfo.Version.Major < 14)
            {
                toastService.ShowError("K přístupnění video chatu je potřeba iOS verze 14.3 a výše");
                return;
            }

            var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
            var microphoneStatus = await Permissions.CheckStatusAsync<Permissions.Microphone>();

            if (cameraStatus != PermissionStatus.Granted)
            {
                cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
                if (cameraStatus != PermissionStatus.Granted)
                {
                    toastService.ShowError("K přístupnění video chatu je povolit přístup ke kameře");
                    return;
                }
            }

            if (microphoneStatus != PermissionStatus.Granted)
            {
                microphoneStatus = await Permissions.RequestAsync<Permissions.Microphone>();
                if (microphoneStatus != PermissionStatus.Granted)
                {
                    toastService.ShowError("K přístupnění video chatu je povolit přístup ke kameře");
                    return;
                }
            }

            await Application.Current.MainPage.Navigation.PushModalAsync(new VideoPage(question.Id));
        }

        private void Current_ModalPopping(object sender, ModalPoppingEventArgs e)
        {
            Dispose();
        }
        private void Dispose()
        {
            chatService.OnMessageRecieved -= ChatService_OnMessageRecieved;
            _ = chatService.Disconnect();
        }
        private async void GoBack()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        public async Task LoadMore()
        {
            Debug.WriteLine(IsBusy);
            if (IsBusy)
                return;

            if (questionRequest.QuestionId == 0)
                return;

            Debug.WriteLine(!oldRepliesLoaded);
            if (!oldRepliesLoaded)
            {
                IsBusy = true;
                questionRequest.Page = questionRequest.Page + 1;
                questionRequest.PageSize = defaultPageSize;

                var lastCommentsTable = await questionsService.GetLastReplies(questionRequest);

                var firstVisibleMessage = Messages.FirstOrDefault();

                if (lastCommentsTable != null)
                {
                    foreach (var comment in lastCommentsTable.Data.OrderByDescending(q => q.CreationDate).ToList())
                    {
                        if (!Messages.Any(m => m.Id == comment.Id))
                        {
                            var newMessage = new QuestionCommentViewModel(comment, userService.UserId);
                            Messages.Insert(0, newMessage);
                        }
                    }

                    oldRepliesLoaded = lastCommentsTable.TotalCount == Messages.Count;
                }
                else
                {
                    toastService.ShowError("Nepodařilo se načíst zprávy");
                }

                IsBusy = false;
            }

        }
        private async Task LoadData()
        {

            try
            {
                chatService = new ChatService(ServiceConfig.authorizationToken);
                await chatService.Init(question.Id);
                chatService.OnMessageRecieved += ChatService_OnMessageRecieved;
            }
            catch (Exception ex)
            {

            }


            IsBusy = true;

            oldRepliesLoaded = false;
            currentPageSize = defaultPageSize;
            questionRequest.Page = 1;
            questionRequest.PageSize = currentPageSize;
            questionRequest.QuestionId = question.Id;

            var lastCommentsTable = await questionsService.GetLastReplies(questionRequest);
            if (lastCommentsTable != null)
            {
                Messages = new ObservableCollection<QuestionCommentViewModel>(lastCommentsTable.Data.OrderBy(q => q.CreationDate).Select(q => new QuestionCommentViewModel(q, userService.UserId)));
                oldRepliesLoaded = lastCommentsTable.TotalCount == Messages.Count;
                MessagingCenter.Instance.Send(this, "OnMessageAdded", Messages.LastOrDefault());
            }
            else
            {
                toastService.ShowError("Nepodařilo se načíst zprávy");
                IsBusy = false;
            }

        }
        private void ChatService_OnMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            QuestionCommentDTO comment = e.Comment;
            var newMessage = new QuestionCommentViewModel
            {
                CreatedAt = comment.CreationDate,
                Message = comment.Text,
                User = comment.LastUpdateUser,
                IsMine = comment.SenderId == userService.UserId
            };
            Messages.Add(newMessage);

            MessagingCenter.Instance.Send(this, "OnMessageAdded", newMessage);
        }

        private void CreateNewMessage()
        {
            NewMessage = new QuestionCommentViewModel();
        }
        private async Task SendMessage()
        {
            if (IsMessageSending)
                return;

            if (String.IsNullOrEmpty(NewMessage.Message))
                return;

            IsMessageSending = true;

            var comment = new QuestionDTO
            {
                Group = question.Id,
                Message = NewMessage.Message
            };

            var oldMessage = NewMessage.Message;

            try
            {
                _ = chatService.SendMessage(comment);
                CreateNewMessage();
            }
            catch (Exception ex)
            {
                NewMessage.Message = oldMessage;
                toastService.ShowError("Nepodařilo se odeslat zprávu. Zkontrolujte své internetové připojení", Enums.NotificationPosition.Top);
            }

            IsMessageSending = false;
        }
    }
}
