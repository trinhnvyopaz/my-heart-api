using Microsoft.AspNetCore.SignalR.Client;
using MyHeart.DTO.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.MobileApp.Services
{
    public class ChatService
    {
        HubConnection hubConnection;
        public delegate void MessageRecievedHandler(object sender, MessageRecievedEventArgs e);
        public event MessageRecievedHandler OnMessageRecieved;
        public ChatService(string jwtToken)
        {
            string url = ServiceConfig.BaseURL;
            string[] split = url.Split("/");
            var filtered = split.Where(s => !String.IsNullOrEmpty(s)).ToList();
            url = $"{filtered[0]}//{filtered[1]}/hub/chat";

            hubConnection = new HubConnectionBuilder()
                .WithUrl(url, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(jwtToken);
                })
                .Build();
        }

        public async Task Init(int questionId)
        {
            await Connect();
            await JoinGroup(questionId);

            hubConnection.On<QuestionCommentDTO>("ReceiveMessage", comment => OnRecieveMessage(comment));
        }

        public async Task Connect()
        {
            await hubConnection.StartAsync();
        }

        public async Task Disconnect()
        {
            await hubConnection.StopAsync();
        }

        public async Task SendMessage(QuestionDTO question)
        {
            await hubConnection.InvokeAsync("SendMessage", question);
        }

        public void OnRecieveMessage(QuestionCommentDTO comment)
        {
            var args = new MessageRecievedEventArgs(comment);
            OnMessageRecieved?.Invoke(this, args);
        }

        public async Task JoinGroup(int questionId)
        {
            await hubConnection.InvokeAsync("JoinGroup", questionId);
        }
    }
    public class MessageRecievedEventArgs
    {
        public QuestionCommentDTO Comment { get; set; }
        public MessageRecievedEventArgs(QuestionCommentDTO comment)
        {
            Comment = comment;
        }
    }
}
