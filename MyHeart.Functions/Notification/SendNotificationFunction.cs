using FirebaseAdmin.Messaging;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MyHeart.Data;
using MyHeart.DTO;
using MyHeart.DTO.User;
using MyHeart.Functions.Utils;
using MyHeart.Services;
using MyHeart.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHeart.Functions.Notification
{
    public class SendNotificationFunction
    {
        private readonly INotificationQueueService _notificationQueueService;
        private readonly FirebaseMessaging _firebaseMessaging;

        public SendNotificationFunction(INotificationQueueService notificationQueueService, FirebaseMessaging firebaseMessaging)
        {
            _notificationQueueService = notificationQueueService;
            _firebaseMessaging = firebaseMessaging;
        }

        [FunctionName("SendPushNotifications")]
        public async Task Run([TimerTrigger("%SendNotificationSchedule%")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var userNotifications = await _notificationQueueService.GetReadyToSendNotifications();
            var notificationResults = new List<NotificationResult>();

            List<Task> sendNotificationsTasks = new List<Task>();

            foreach (var userNotification in userNotifications)
            {
                try
                {
                    var messages = CreateMulticastMessages(userNotification);
                    sendNotificationsTasks.Add(SendNotifications(log, messages, notificationResults));

                }
                catch (Exception ex)
                {
                    log.LogError(ex, "sending notification failed");
                }


            }

            await Task.WhenAll(sendNotificationsTasks);

            await _notificationQueueService.UpdateSendedNotificationByIds(notificationResults);
        }

        private List<KeyValuePair<int, MulticastMessage>> CreateMulticastMessages(UserNotificationsDTO userNotification)
        {
            return userNotification.Notifications.Select(notification => new KeyValuePair<int, MulticastMessage>(
                notification.Id,
                new MulticastMessage
                {
                    Tokens = notification.User.UserFmcTokens.Select(t => t.Token).ToList(),
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = notification.Title,
                        Body = notification.Description,
                    },
                    Data = notification.CustomData
                })).ToList();
        }


        private async Task SendNotifications(ILogger log, List<KeyValuePair<int, MulticastMessage>> messages, List<NotificationResult> queueIdsToUpdate)
        {
            var sendNotificationTasks = messages.Select(message => SendNotification(log, message));
            var notificationIds = await Task.WhenAll(sendNotificationTasks);

            queueIdsToUpdate.AddRange(notificationIds.ToList());
        }


        private async Task<NotificationResult> SendNotification(ILogger log, KeyValuePair<int, MulticastMessage> message)
        {
            var result = new NotificationResult
            {
                Id = message.Key
            };

            if (message.Value.Tokens.Count == 0)
            {
                result.Error = "No fmc token";
                return result;
            }


            var response = await _firebaseMessaging.SendMulticastAsync(message.Value);

            if (response.SuccessCount > 0)
            {
                log.LogInformation($"Successfully sent {response.SuccessCount} messages for SID {message.Key}:");
                return result;
            }
            else
            {
                log.LogWarning($"Failed to send message for SID {message.Key}:");
                var errorResponses = response.Responses.Where(x => x.Exception != null);
                result.Error = "";
                foreach (var error in errorResponses)
                {
                    result.Error += $"error code: {error.Exception.ErrorCode} messeging code: {error.Exception.MessagingErrorCode} message: {error.Exception.Message}";
                }

                return result;
            }
        }
    }
}
