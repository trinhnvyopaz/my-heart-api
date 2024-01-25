using MyHeart.DTO.Questions;
using MyHeart.MobileApp.ViewModels;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyHeart.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        public ChatPage(QuestionListDTO question)
        {
            BindingContext = new ChatPageViewModel(question);
            InitializeComponent();
            MessagingCenter.Instance.Subscribe<ChatPageViewModel, QuestionCommentViewModel>(this, "OnMessageAdded", async (sender, message) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    MessagesCollectionView.ScrollTo(message, null, ScrollToPosition.MakeVisible, false);
                    ViewModel.IsBusy = false;
                });              
            });

            //MessagingCenter.Instance.Subscribe<ChatPageViewModel, QuestionCommentViewModel>(this, "KeepScroll", async (sender, message) =>
            //{
            //    Device.BeginInvokeOnMainThread(async () =>
            //    {
            //        await Task.Delay(100);
            //        MessagesCollectionView.ScrollTo(message, ScrollToPosition.Start, false);
            //        ViewModel.IsBusy = false;
            //    });
            //});
        }

        private ChatPageViewModel ViewModel => BindingContext as ChatPageViewModel;

        private void MessagesCollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            Debug.WriteLine(e.FirstVisibleItemIndex);
            if (e.FirstVisibleItemIndex == 0)
            {
                _ = ViewModel.LoadMore();
            }

        }
    }
}