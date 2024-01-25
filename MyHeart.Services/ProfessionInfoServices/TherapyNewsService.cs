using AutoMapper;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO;
using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.Security.Claims;
using MyHeart.DTO.Notification;

namespace MyHeart.Services
{
    public class TherapyNewsService : ITherapyNewsService
    {


        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MyHeartContext _myHeartContext;
        private readonly IConfiguration _configuration;

        private readonly IEmailService _emailService;
        private readonly INotificationQueueService _notificationQueueService;

        public TherapyNewsService(IHttpContextAccessor httpContextAccessor, MyHeartContext gradologyContext, IMapper mapper, IConfiguration configuration, IEmailService emailService, INotificationQueueService notificationQueueService)
        {
            _httpContextAccessor = httpContextAccessor;
            _myHeartContext = gradologyContext;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
            _notificationQueueService = notificationQueueService;
        }


        public async Task<IEnumerable<TherapyNewsDTO>> TherapyNewsListAsync()
        {
            var therapyNews = await _myHeartContext.TherapyNews.ToListAsync();

            return _mapper.Map<IEnumerable<TherapyNews>, IEnumerable<TherapyNewsDTO>>(therapyNews);
        }


        public async Task<TherapyNewsDTO> TherapyNewAsync(int therapyNewsId)
        {
            var therapyNews = await _myHeartContext.TherapyNews
                .Where(u => u.Id == therapyNewsId)
                .Include(p => p.Diseases)
                .FirstOrDefaultAsync();

            return _mapper.Map<TherapyNews, TherapyNewsDTO>(therapyNews);
        }

        public async Task<TherapyNewsDTO> UpdateTherapyNews(TherapyNewsDTO therapyNews, string user)
        {
            var dbtherapyNews = await _myHeartContext.TherapyNews
                                        .Where(u => u.Id == therapyNews.Id)
                                        .Include(p => p.Diseases)
                                        .FirstOrDefaultAsync();
            if (dbtherapyNews == null)
            {
                return null;
            }

            dbtherapyNews.Text = therapyNews.Text;
            dbtherapyNews.WebLink = therapyNews.WebLink;
            dbtherapyNews.Description = therapyNews.Description;
            dbtherapyNews.LastUpdateUser = user;

            #region disease bond
            int bondCount = dbtherapyNews.Diseases == null ? 0 : dbtherapyNews.Diseases.Count;
            List<int> idList = new List<int>();
            List<TherapyNews_Disease_Sick> diseaseList = new List<TherapyNews_Disease_Sick>();
            var lastDisease = await _myHeartContext.TherapyNews_Disease_Sick.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastDisease == null ? 0 : lastDisease.Id;
            int itemId = 1;
            foreach (var item in therapyNews.Diseases)
            {
                if (item.Id == 0)
                {
                    if (lastDisease != null)
                    {
                        lastId++;
                        item.Id = lastId;
                    }
                    else
                    {
                        item.Id = itemId;
                        itemId++;
                    }
                    bool exist = false;
                    foreach (var existItem in dbtherapyNews.Diseases)
                    {
                        if (existItem.TherapyNewsId == item.TherapyNewsId && existItem.DiseaseId == item.DiseaseId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        diseaseList.Add(_mapper.Map<TherapyNews_Disease_Sick>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<TherapyNews_Disease_Sick>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.TherapyNews_Disease_Sick.AddAsync(bond);
                        idList.Add(item.Id);
                    }

                }
                else
                {
                    diseaseList.Add(_mapper.Map<TherapyNews_Disease_Sick>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbtherapyNews.Diseases)
                {
                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.TherapyNews_Disease_Sick.Remove(item);
                    }
                }
            }
            #endregion

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<TherapyNewsDTO>(therapyNews);
        }

        public async Task<TherapyNewsDTO> AddTherapyNewsAsync(TherapyNewsDTO therapyNews, string user)
        {
            var dbtherapyNews = new TherapyNews()
            {
                Text = therapyNews.Text,
                Description = therapyNews.Description,
                WebLink = therapyNews.WebLink,
                CreateDate = DateTime.Now,
                LastUpdateUser = user
            };

            _myHeartContext.Add(dbtherapyNews);

            await _myHeartContext.SaveChangesAsync();

            #region disease bond
            var lastDisease = await _myHeartContext.TherapyNews_Disease_Sick.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastDisease == null ? 0 : lastDisease.Id;
            int itemId = 1;
            if (therapyNews.Diseases != null)
            {
                foreach (var item in therapyNews.Diseases)
                {
                    if (item.Id == 0)
                    {
                        if (lastDisease != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.TherapyNewsId = dbtherapyNews.Id;
                        item.LastUpdateUser = user;
                        await _myHeartContext.TherapyNews_Disease_Sick.AddAsync(_mapper.Map<TherapyNews_Disease_Sick>(item));
                    }
                }
            }
            #endregion

            await _myHeartContext.SaveChangesAsync();

            await SendNotifications(dbtherapyNews);



            return _mapper.Map<TherapyNews, TherapyNewsDTO>(dbtherapyNews);
        }

        private async Task SendNotifications(TherapyNews news)
        {

            //people that take all emails
            var recipientsTakingAll = await _myHeartContext.Users.Where(x => x.EmailComfirmed && x.SubscriptionPreferences == 0).ToListAsync();

            //people that take emails only for their disease
            var recipientsTakingOnlyTheirs = await _myHeartContext.Users.Where(x => x.EmailComfirmed && x.TherapyNewsEmailNotification == true && x.SubscriptionPreferences == 1).Include(x => x.DiagnosedDiseases).ToListAsync();
            var filteredRecipients = recipientsTakingOnlyTheirs.Where(x => x.DiagnosedDiseases.Any(y => news.Diseases.Any(z => z.DiseaseId == y.DiseaseId))).ToList();

            var allUsers = recipientsTakingOnlyTheirs.Concat(recipientsTakingAll);

            var notificationQueueList = new List<NotificationQueueDTO>();

            foreach (var user in allUsers)
            {
                if (user.TherapyNewsEmailNotification)
                {
                    _ = SendEmail(user.Email, news);
                }
                else
                {
                    Dictionary<string, string> customData = new Dictionary<string, string>
                    {
                        { "Id", $"{news.Id}" },
                        { "Type", NotificationTypes.NEWS }
                    };

                    var notificationQueue = new NotificationQueueDTO
                    {
                        ScheduledAt = DateTime.Now,
                        Title = "Novinka v léčbě",
                        Description = news.Text,
                        UserId = user.Id,
                        CustomData = customData
                    };

                    notificationQueueList.Add(notificationQueue);
                }
            }

            await _notificationQueueService.CreateNotificationQueue(notificationQueueList);
        }

        public async Task<bool> SendEmail(string email, TherapyNews news)
        {
            var message =
                $"Dobrý den,<br /> v aplikac Moje Srdce byla vytvořena nová novinka o v léčbě:<br /> {news.Text}.<br /><br /> Nastavení odebíraní emailů můžete změnit ve vašem profilu.<br /><br /> S pozdravem,<br /><br /> Tým Moje Srdce";
            var subject = "Nová novinka v léčbě";

            _emailService.SendMail(email, subject, message);

            return true;
        }

        public async Task<TherapyNewsDTO> DeleteTherapyNews(int therapyNewsId)
        {
            var dbtherapyNews = await _myHeartContext.TherapyNews.FirstOrDefaultAsync(u => u.Id == therapyNewsId);

            if (dbtherapyNews == null)
            {
                return null;
            }

            _myHeartContext.TherapyNews.Remove(dbtherapyNews);

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<TherapyNewsDTO>(dbtherapyNews);
        }


        public async Task<Dictionary<string, string>> ValidateTherapyNews(TherapyNewsDTO therapyNews)
        {
            var errorList = new Dictionary<string, string>();

            if (therapyNews.Text.Length < 2)
                errorList.Add("Text", "Text nesmí být kratší než 2 znaky");

            return errorList;
        }

        public async Task<DataTableResponse<TherapyNewsDTO>> GetTherapyNewsTable(DataTableRequest request)
        {
            List<TherapyNews> symptoms;
            int totalCount;

            //no need to search all strings if there is no filter
            if (!string.IsNullOrEmpty(request.Filter))
            {
                //no need to filter twice
                var filtered = _myHeartContext.TherapyNews
                    .Where(x => x.Text.ToLower()
                    .Contains(request.Filter.ToLower()));

                symptoms = await filtered
                    .ApplyOrdering(request.ColumnOrders)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                totalCount = await filtered
                    .CountAsync();
            }
            else
            {
                symptoms = await _myHeartContext.TherapyNews
                    .ApplyOrdering(request.ColumnOrders)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                totalCount = await _myHeartContext.TherapyNews
                    .CountAsync();
            }

            return new DataTableResponse<TherapyNewsDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<TherapyNews>, IEnumerable<TherapyNewsDTO>>(symptoms).ToList()
            };
        }

        public TherapyNewsSubscriptionSettingsDto GetSubscriptionPreferences()
        {
            var user = GetCurrentUser();

            return GetSubscriptionPreferences(user);
        }

        public TherapyNewsSubscriptionSettingsDto GetSubscriptionPreferences(int id)
        {
            var user = _myHeartContext.Users.Find(id);

            return GetSubscriptionPreferences(user);
        }

        private TherapyNewsSubscriptionSettingsDto GetSubscriptionPreferences(User user)
        {
            if (user == null)
                return null;

            return new TherapyNewsSubscriptionSettingsDto
            {
                UserId = user.Id,
                SubscriptionPreferences = user.SubscriptionPreferences,
                TherapyNewsEmailNotification = user.TherapyNewsEmailNotification
            };
        }

        public async Task<TherapyNewsSubscriptionSettingsDto> UpdateSubscriptionPreferences(TherapyNewsSubscriptionSettingsDto model, string user)
        {
            var currentUser = await _myHeartContext.Users
                .Where(x => x.Id == model.UserId)
                .FirstOrDefaultAsync();

            if (currentUser == null)
                return null;

            currentUser.SubscriptionPreferences = model.SubscriptionPreferences;
            currentUser.TherapyNewsEmailNotification = model.TherapyNewsEmailNotification;
            currentUser.LastUpdateUser = user;

            _myHeartContext.Entry(currentUser).State = EntityState.Modified;
            await _myHeartContext.SaveChangesAsync();

            return model;
        }

        private User GetCurrentUser()
        {
            if (_httpContextAccessor.HttpContext.User == null)
                return null;

            var userIdString = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdString, out int userId))
                return null;

            var user = _myHeartContext.Users.Find(userId);

            if (user == null)
                return null;

            return user;
        }
    }
}