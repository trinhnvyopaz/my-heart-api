using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MyHeart.Api.Util;
using MyHeart.DTO.Questions;
using MyHeart.DTO.User;
using MyHeart.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHeart.Api.Hubs {
    public class ChatHub : Hub{
        /*IUserService _userService;
        IQuestionService _questionService;*/
        IServiceScopeFactory _scopeFactory;

        public ChatHub(IServiceScopeFactory scopeFactory) {
            _scopeFactory = scopeFactory;

            
        }

        [HubMethodName("SendMessage")]
        public async Task<QuestionCommentDTO?> SendMessage(QuestionDTO message) {
            var scope = _scopeFactory.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var userService = serviceProvider.GetService<IUserService>();
            var questionService = serviceProvider.GetService<IQuestionService>();

            var newComment = new QuestionCommentDTO();

            newComment.CreationDate = DateTime.Now;

            var sender = await userService.CurrentUserAsync();
            newComment.LastUpdateUser = $"{sender.FirstName} {sender.LastName}";
            newComment.SenderId = sender.Id;
            newComment.Text = message.Message;

            newComment.QuestionId = message.Group;

            var questionRes = await questionService.ReplyToQuestion(newComment);
            if (questionRes == null) {
                return null;
            }

            var tasks = new List<Task> {
                //Send
                Clients.Group(questionRes.QuestionId.ToString()).SendAsync("ReceiveMessage", questionRes)
            };

            await Task.WhenAll(tasks);

            scope.Dispose();

            return questionRes;
        }

        [HubMethodName("SendMessageForUser")]
        [Authorize(Policy = Policies.MinAdmin)]
        public async Task<QuestionCommentDTO?> SendMessage(ImpersonatedQuestionDTO message) {
            //nesmi se psat za ostatni lidi prej
            return null;

            /*var scope = _scopeFactory.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var userService = serviceProvider.GetService<IUserService>();
            var questionService = serviceProvider.GetService<IQuestionService>();

            var newComment = new QuestionCommentDTO();

            newComment.CreationDate = DateTime.Now;

            var sender = await userService.UserByIdAsync(message.UserId);
            var sentBy = await userService.CurrentUserAsync();
            newComment.LastUpdateUser = $"{sentBy.FirstName} {sentBy.LastName}";
            newComment.SenderId = sender.Id;
            newComment.Text = message.Message;

            newComment.QuestionId = message.Group;

            var questionRes = await questionService.ReplyToQuestion(newComment);
            if (questionRes == null) {
                return null;
            }

            var tasks = new List<Task> {

                //Send
                Clients.Group(questionRes.QuestionId.ToString()).SendAsync("ReceiveMessage", questionRes)
            };

            await Task.WhenAll(tasks);

            scope.Dispose();

            return questionRes;*/
        }

        [HubMethodName("JoinGroup")]
        public async Task<bool> JoinGroup(int questionId) {
            var scope = _scopeFactory.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var userService = serviceProvider.GetService<IUserService>();
            var questionService = serviceProvider.GetService<IQuestionService>();

            var user = await userService.CurrentUserAsync();

            if (user.UserType == UType.Patient && !(await questionService.DoesQuestionBelongToUser(questionId, user.Id))) {
                return false;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, questionId.ToString());

            scope.Dispose();

            return true;
        }
    }
}
