using MyHeart.DTO.Questions;
using MyHeart.MobileApp.Services;
using MyHeart.MobileApp.ViewModels;
using MyHeart.MobileApp.ViewModels.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Views.Controls
{
    public class VideoWebView : AutoHeigthWebView
    {
        public event EventHandler<EventArgs> Disconnected;

        VideoRoom room;

        public VideoWebView()
        {
            MessagingCenter.Instance.Subscribe<VideoPageViewModel, InitVideoMessage>(this, "InitVideoChat", (sender, message) => InitVideo(message));

            Navigating += OnWebViewNavigating;
        }

        void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            var url = e.Url;
            if (url.StartsWith("invoke://"))
            {
                e.Cancel = true; // cancel the navigation
                                 // Extract the parameter from the URL and handle it.
                var param = url.Split('=')[1];

                if (param == "end")
                {
                    OnDisconnected();
                }
            }
        }

        public void OnDisconnected()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);            
        }

        private async Task InitVideo(InitVideoMessage message)
        {

            MessagingCenter.Instance.Unsubscribe<VideoPageViewModel, VideoRoomDTO>(this, "InitVideoChat");

            this.room = message.VideoRoom;

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(VideoWebView)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("MyHeart.MobileApp.Resources.video.html");

            using (var reader = new StreamReader(stream))
            {
                string html = reader.ReadToEnd();
                Source = new HtmlWebViewSource { Html = html };
            }

            await Task.Delay(500);

            var initVideo = $"setRoomDetail('{room.AccessToken}','{room.Name}');";
            await EvaluateJavaScriptAsync(initVideo);

            await EvaluateJavaScriptAsync("joinRoom()");
        }
    }
}
