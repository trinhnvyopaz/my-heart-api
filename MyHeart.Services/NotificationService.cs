using AutoMapper;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.DTO;
using MyHeart.DTO.Notification;
using MyHeart.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notification = MyHeart.Data.Models.Notification;

namespace MyHeart.Services
{
    public class NotificationService : INotificationService
    {

        private readonly MyHeartContext _context;
        private readonly IMapper _mapper;
        private readonly FirebaseMessaging _firebaseMessaging;

        public NotificationService(MyHeartContext context, IMapper mapper, FirebaseMessaging firebaseMessaging)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper;
            _firebaseMessaging = firebaseMessaging;
        }

        public async Task<NotificationDto> CreateNotification(NotificationDto model)
        {
            if (model == null)
                return null;

            var dbModel = new Notification
            {
                DiseaseId = model.DiseaseId,
                Name = model.Name,
                Description = model.Description
            };

            _context.Notifications
                .Add(dbModel);
            await _context.SaveChangesAsync();

            return _mapper.Map<Notification, NotificationDto>(dbModel);
        }

        public async Task<bool> SendNotification(int userId, string title, string body, Dictionary<string, string> customData)
        {
            var user = await _context.Users
                .Include(x => x.UserFmcTokens)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null || user.UserFmcTokens.Count == 0)
                return false;


            var message = new MulticastMessage
            {
                Tokens = user.UserFmcTokens.Select(x => x.Token).ToList(),
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body,
                },
                Data = customData
            };

            await _firebaseMessaging.SendMulticastAsync(message);

            return true;
        }
    }
}
