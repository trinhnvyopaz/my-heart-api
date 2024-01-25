using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MyHeart.Data;
using Microsoft.EntityFrameworkCore;
using MyHeart.Services.Interfaces;
using MyHeart.Data.Models;
using System.Linq;
using MyHeart.Services;
using AutoMapper;
using MyHeart.DTO.Notification;
using System.Collections.Generic;
using AngleSharp.Css.Dom;
using MyHeart.Functions.Utils;
using Newtonsoft.Json;

namespace MyHeart.Functions.Notification
{
    public class NotificationFunction
    {
        private MyHeartContext _myHeartContext;

        private IAnamnesisService _anamnesisService;
        private INotificationQueueService _notificationQueueService;
        private IMapper _mapper;

        const int NotificationPeriod = 7;
        const int NotificationScheduleHour = 9;
        const int NotificationTimeSendFrom = 9;
        const int NotificationTimeSendTo = 20;
        const int MinRangeBetweenNotifications = 1;

        public NotificationFunction(MyHeartContext myHeartContext, IAnamnesisService anamnesisService, INotificationQueueService notificationQueueService, IMapper mapper)
        {
            _myHeartContext = myHeartContext;
            _anamnesisService = anamnesisService;
            _notificationQueueService = notificationQueueService;
            _mapper = mapper;
        }

        [FunctionName("GenerateSuggestionNotificationQueue")]
        public async Task Run([TimerTrigger("%NotificationQueueSchedule%")] TimerInfo myTimer, ILogger log)
        {

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");

            var now = DateTime.Now.Date;

            var alreadyCreated = await _myHeartContext.NotificationQueueLog.AnyAsync(l => now.Year == l.CreateAt.Year && now.Month == l.CreateAt.Month);

            if (!alreadyCreated)
            {
                var userDiseasesList = await _anamnesisService.GetAllDiseaseAnamneses();

                var userDiseaseGroups = userDiseasesList
                    .Where(d => !string.IsNullOrEmpty(d.Disease.SystemicMeasures))
                    .Where(d => d.User.SubscriptionPreferences != 2)
                    .GroupBy(u => u.UserId)
                    .ToList();


                foreach (var userDiseaseGroup in userDiseaseGroups)
                {
                    try
                    {
                        var lastSchedule = now.AddHours(NotificationScheduleHour);

                        var notifications = await _notificationQueueService.GetNotificationsByUserId(userDiseaseGroup.Key);

                        foreach (var userDisease in userDiseaseGroup)
                        {
                            var currentSchedule = lastSchedule;

                            var scheduleOverlap = notifications.Any(n => n.ScheduledAt == currentSchedule);
                            if (scheduleOverlap)
                            {
                                currentSchedule = FindeSchedule(notifications, currentSchedule);
                            }

                            Dictionary<string, string> customData = new Dictionary<string, string>
                            {
                                { "Type", NotificationTypes.SystematicMeasure }
                            };

                            var queue = new NotificationQueue
                            {
                                ScheduledAt = currentSchedule,
                                Title = userDisease.Disease.Name,
                                Description = TextHelper.ExtractTextFromHtml(userDisease.Disease.SystemicMeasures),
                                UserId = userDisease.UserId,
                                CustomData = JsonConvert.SerializeObject(customData)
                            };

                            _myHeartContext.Add(queue);
                            notifications.Add(_mapper.Map<NotificationQueueDTO>(queue));

                            lastSchedule = lastSchedule.AddDays(NotificationPeriod);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                _myHeartContext.NotificationQueueLog.Add(new NotificationQueueLog
                {
                    CreateAt = now
                });

                await _myHeartContext.SaveChangesAsync();
            }
        }

        public DateTime FindeSchedule(List<NotificationQueueDTO> notifications, DateTime currentSchedule)
        {
            DateTime newSchedule = new DateTime();

            bool validSchedule = false;
            while (!validSchedule)
            {
                newSchedule = currentSchedule.AddHours(MinRangeBetweenNotifications);
                bool scheduleOverlap = notifications.Any(n => n.ScheduledAt == newSchedule);
                if (!scheduleOverlap && newSchedule.Hour >= NotificationTimeSendFrom && newSchedule.Hour <= NotificationTimeSendTo)
                {
                    validSchedule = true;
                }
            }

            return newSchedule;
        }
    }
}
