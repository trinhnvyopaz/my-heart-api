using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.DTO;
using MyHeart.DTO.Notification;
using MyHeart.DTO.User;
using MyHeart.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services
{
    public class NotificationQueueService : INotificationQueueService
    {
        private MyHeartContext _myHeartContext;
        private IMapper _mapper;

        public NotificationQueueService(MyHeartContext myHeartContext, IMapper mapper)
        {
            _myHeartContext = myHeartContext;
            _mapper = mapper;
        }

        public async Task<List<NotificationQueueDTO>> CreateNotificationQueue(List<NotificationQueueDTO> notificationQueue)
        {
            foreach (var item in notificationQueue)
            {
                var dbQueue = new NotificationQueue
                {
                    Description = item.Description,
                    ScheduledAt = item.ScheduledAt,
                    Title = item.Title,
                    UserId = item.UserId,
                    CustomData = JsonConvert.SerializeObject(item.CustomData)
                };

                _myHeartContext.Add(dbQueue);
            }

            await _myHeartContext.SaveChangesAsync();


            return notificationQueue;
        }

        public async Task<List<UserNotificationsDTO>> GetReadyToSendNotifications()
        {
            var now = DateTime.Now;

            var notificationQueue = await _myHeartContext.NotificationQueue
                .Include(q => q.User)
                    .ThenInclude(u => u.UserFmcTokens)
                .Where(q => !q.Sended)
                .Where(q => q.ScheduledAt <= now)
                .ToListAsync();

            var groupedNotifications = notificationQueue
                .GroupBy(q => q.UserId);

            var userNotifications = groupedNotifications
                .Select(g => new UserNotificationsDTO
                {
                    UserId = g.Key,
                    Notifications = g.Select(nq => _mapper.Map<NotificationQueueDTO>(nq)).ToList()
                })
                .ToList();

            return userNotifications;
        }

        public async Task<List<NotificationQueueDTO>> GetNotificationsByUserId(int id)
        {
            var now = DateTime.Now;

            var notificationQueue = await _myHeartContext.NotificationQueue
                .Where(q => !q.Sended)
                .Where(q => q.UserId == id)
                .ToListAsync();

            return notificationQueue.Select(_mapper.Map<NotificationQueueDTO>).ToList();
        }

        public async Task UpdateSendedNotificationByIds(List<NotificationResult> notificationResults)
        {
            int[] ids = notificationResults.Select(q => q.Id).ToArray();

            var queue = await _myHeartContext.NotificationQueue.Where(q => ids.Contains(q.Id)).ToListAsync();

            foreach (var item in queue)
            {
                var result = notificationResults.First(x => x.Id == item.Id);
                item.Sended = true;
                item.Error = result.Error;
            }

            await _myHeartContext.SaveChangesAsync();
        }
    }
}
