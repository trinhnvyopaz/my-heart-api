using AutoMapper;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.Services.Interfaces;
using MyHeart.DTO.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyHeart.DTO;
using MyHeart.DTO.Questions;
using System.Security.Cryptography;
using System.Collections;
using MyHeart.DTO.Notification;

namespace MyHeart.Services
{
    public class QuestionService : IQuestionService
    {
        private MyHeartContext _myheartContext;
        private IMapper _mapper;
        private ISanitizer _sanitizer;
        private INotificationService _notificationService;

        public QuestionService(MyHeartContext context, IMapper mapper, ISanitizer sanitizer, INotificationService notificationService)
        {
            _myheartContext = context;
            _mapper = mapper;
            _sanitizer = sanitizer;
            _notificationService = notificationService;
        }

        public async Task<List<QuestionListDTO>> GetListAsync()
        {
            var list = await _myheartContext.Questions
                .Select(q => new QuestionListDTO
                {
                    Id = q.Id,
                    Subject = q.Subject,
                    Status = q.Status,
                    CreationDate = q.CreationDate.ToString("dd-MM-yyyy"),
                    UserId = q.UserId,
                    Name = q.User.FirstName + " " + q.User.LastName,
                    LastUpdateDate = q.LastUpdateDate,
                    LastUpdateUser = q.LastUpdateUser
                }).ToListAsync();

            return list;
        }

        public async Task<IEnumerable<QuestionFullDto>> GetListFullAsync()
        {
            var list = await _myheartContext.Questions
                .Include(q => q.User)
                .Include(q => q.Comments)
                    .ThenInclude(c => c.User)
                .ToListAsync();

            return list.Select(_mapper.Map<QuestionFullDto>);
        }

        public async Task<List<QuestionListDTO>> GetOpenUserQuestions(int userId)
        {
            return await _myheartContext.Questions.Where(x => x.UserId == userId && x.Status != QuestionStatus.Closed)
                .Include(x => x.User)
                .Select(q => new QuestionListDTO
                {
                    Id = q.Id,
                    Subject = q.Subject,
                    Status = q.Status,
                    CreationDate = q.CreationDate.ToString("dd-MM-yyyy"),
                    UserId = q.UserId,
                    Name = q.User.FirstName + " " + q.User.LastName,
                    LastUpdateDate = q.LastUpdateDate,
                    LastUpdateUser = q.LastUpdateUser
                }).ToListAsync();
        }
        public async Task<List<QuestionListDTO>> GetClosedUserQuestions(int userId)
        {
            return await _myheartContext.Questions.Where(x => x.UserId == userId && x.Status == QuestionStatus.Closed)
                .Include(x => x.User)
                .Select(q => new QuestionListDTO
                {
                    Id = q.Id,
                    Subject = q.Subject,
                    Status = q.Status,
                    CreationDate = q.CreationDate.ToString("dd-MM-yyyy"),
                    UserId = q.UserId,
                    Name = q.User.FirstName + " " + q.User.LastName,
                    LastUpdateDate = q.LastUpdateDate,
                    LastUpdateUser = q.LastUpdateUser
                }).ToListAsync();
        }

        public async Task<DataTableResponse<QuestionListDTO>> GetOpenUserQuestionsDataTable(int userId, DataTableRequest request)
        {
            var questions = _myheartContext.Questions
                .Include(x => x.Comments)
                .Where(x => (x.UserId == userId || x.Comments.Any(c => c.UserId == userId)) && x.Status != QuestionStatus.Closed );

            if (!string.IsNullOrEmpty(request.Filter))
            {
                questions.Where(x => x.Subject.ToLower().Contains(request.Filter.ToLower()));
            }

            var totalCount = await questions.CountAsync();

            var selected = await questions.Include(x => x.User)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(q => new QuestionListDTO
                {
                    Id = q.Id,
                    Subject = q.Subject,
                    Status = q.Status,
                    CreationDate = q.CreationDate.ToString("dd-MM-yyyy"),
                    UserId = q.UserId,
                    Name = q.User.FirstName + " " + q.User.LastName,
                    LastUpdateDate = q.LastUpdateDate,
                    LastUpdateUser = q.LastUpdateUser
                }).ToListAsync();

            return new DataTableResponse<QuestionListDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = selected
            };
        }

        public async Task<DataTableResponse<QuestionListDTO>> GetClosedUserQuestionsDataTable(int userId, DataTableRequest request)
        {
            var questions = _myheartContext.Questions.Where(x => x.UserId == userId && x.Status == QuestionStatus.Closed);

            if (!string.IsNullOrEmpty(request.Filter))
            {
                questions.Where(x => x.Subject.ToLower().Contains(request.Filter.ToLower()));
            }

            var totalCount = await questions.CountAsync();

            var selected = await questions.Include(x => x.User)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(q => new QuestionListDTO
                {
                    Id = q.Id,
                    Subject = q.Subject,
                    Status = q.Status,
                    CreationDate = q.CreationDate.ToString("dd-MM-yyyy"),
                    UserId = q.UserId,
                    Name = q.User.FirstName + " " + q.User.LastName,
                    LastUpdateDate = q.LastUpdateDate,
                    LastUpdateUser = q.LastUpdateUser
                }).ToListAsync();

            return new DataTableResponse<QuestionListDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = selected
            };
        }

        public async Task<QuestionFullDto> GetQuestionById(int questionId, bool includeQuestions = false)
        {
            var question = await _myheartContext.Questions.Where(x => x.Id == questionId).Include(x => x.User).FirstOrDefaultAsync();

            if (question == null)
            {
                return null;
            }

            var body = (await _myheartContext.QuestionComments.Where(x => x.QuestionId == questionId)
                .OrderBy(x => x.CreationDate).FirstOrDefaultAsync());

            var text = "";
            if (body != null)
            {
                text = body.Text;
            }

            List<QuestionComment> comments = null;
            if (includeQuestions)
            {
                comments = await _myheartContext.QuestionComments.Where(c => c.QuestionId == questionId).ToListAsync();
            }

            return new QuestionFullDto()
            {
                Id = question.Id,
                UserId = question.UserId,
                Subject = question.Subject,
                Body = text,
                Name = question.User.FirstName + " " + question.User.LastName,
                CreationDate = question.CreationDate.ToString("dd-MM-yyyy"),
                Status = question.Status,
                LastUpdateDate = question.LastUpdateDate,
                LastUpdateUser = question.LastUpdateUser,
                Comments = comments?.Select(_mapper.Map<QuestionCommentFullDTO>)
            };
        }

        public async Task<QuestionListDTO> AskQuestion(int userId, QuestionListDTO question, string userName)
        {
            var dbUser = await _myheartContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();

            var dbQuestion = new Question()
            {
                Subject = question.Subject,
                UserId = userId,
                CreationDate = DateTime.Now,
                LastUpdateUser = userName,
                Status = QuestionStatus.Open
            };

            await _myheartContext.Questions.AddAsync(dbQuestion);

            await _myheartContext.SaveChangesAsync();

            var firstReply = new QuestionComment()
            {
                CreationDate = DateTime.Now,
                LastUpdateUser = userName,
                QuestionId = dbQuestion.Id,
                Text = question.Body,
                UserId = dbUser.Id
            };

            await _myheartContext.QuestionComments.AddAsync(firstReply);

            await _myheartContext.SaveChangesAsync();

            dbQuestion.User = dbUser;

            return new QuestionListDTO()
            {
                Id = dbQuestion.Id,
                UserId = dbQuestion.UserId,
                Subject = dbQuestion.Subject,
                Body = firstReply.Text,
                Name = dbQuestion.User.FirstName + " " + dbQuestion.User.LastName,
                CreationDate = dbQuestion.CreationDate.ToString("dd-MM-yyyy"),
                Status = dbQuestion.Status,
                LastUpdateDate = dbQuestion.LastUpdateDate,
                LastUpdateUser = dbQuestion.LastUpdateUser
            };
        }

        public async Task<QuestionCommentDTO> ReplyToQuestion(QuestionCommentDTO reply)
        {
            reply.Text = _sanitizer.SanitizeHtml(reply.Text);

            var dbQuestion = await _myheartContext.Questions
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == reply.QuestionId);

            if (!(await ValidateMessage(reply, dbQuestion)))
            {
                return null;
            }

            var user = await _myheartContext.Users.Where(x => x.Id == reply.SenderId).FirstOrDefaultAsync();

            var DbReply = new QuestionComment
            {
                CreationDate = reply.CreationDate,
                QuestionId = reply.QuestionId,
                Text = reply.Text,
                UserId = reply.SenderId,
                LastUpdateUser = $"{user.FirstName} {user.LastName}"
            };

            dbQuestion.Status = reply.SenderId == dbQuestion.UserId ? QuestionStatus.AwaitingDoctorResponse : QuestionStatus.AwaitingPatientResponse;

            await _myheartContext.AddAsync(DbReply);

            await _myheartContext.SaveChangesAsync();

            if (reply.SenderId != dbQuestion.UserId)
            {
                var customData = new Dictionary<string, string>
                {
                    { "Type", NotificationTypes.QuestionAnswer},
                    { "Id", $"{dbQuestion.Id }"}
                };

                await _notificationService.SendNotification(userId: dbQuestion.UserId, title: dbQuestion.Subject, body: DbReply.Text, customData: customData);
            }

            return reply;
        }

        private async Task<bool> ValidateMessage(QuestionCommentDTO reply, Question question)
        {
            if (question == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(reply.Text))
            {
                return false;
            }

            if (reply.Id >= 0 && (await _myheartContext.QuestionComments.Where(x => x.Id == reply.Id).FirstOrDefaultAsync() != null))
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DoesQuestionBelongToUser(int questionId, int userId)
        {
            var dbUser = await _myheartContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            var dbQuestion = await _myheartContext.Questions.Where(x => x.Id == questionId).FirstOrDefaultAsync();

            if (dbUser == null || dbQuestion == null)
            {
                return false;
            }

            return dbUser.Id == dbQuestion.UserId;
        }

        public async Task<bool> ArchiveQuestion(int QuestionId, UserDTO user)
        {
            var dbQuestion = await _myheartContext.Questions.Where(x => x.Id == QuestionId).FirstOrDefaultAsync();

            if (dbQuestion == null)
            {
                return false;
            }

            dbQuestion.Status = QuestionStatus.Closed;
            dbQuestion.LastUpdateUser = user.FirstName + " " + user.LastName;

            await _myheartContext.SaveChangesAsync();

            return true;
        }

        public async Task<DataTableResponse<QuestionListDTO>> GetQuestionsTable(GroupedDataTableRequest request)
        {
            List<QuestionListDTO> questions;
            int totalCount;

            //no need to search all strings if there is no filter
            if (!string.IsNullOrEmpty(request.Filter) || request.Groups.Any())
            {
                var statuses = request.Groups.Select(s => (QuestionStatus)s).ToList();

                var usersName = request.Filter.Trim().Split(" ");
                IQueryable<Question> filtered;

                if (usersName.Length == 1)
                {
                    filtered = _myheartContext.Questions
                        .Where(x => (x.Subject.ToLower()
                        .Contains(request.Filter.ToLower()) || x.User.FirstName.ToLower().Contains(usersName[0].ToLower()) || x.User.LastName.ToLower().Contains(usersName[0].ToLower())) && (!statuses.Any() || statuses.Contains(x.Status)));
                }
                else if (usersName.Length == 2)
                {
                    filtered = _myheartContext.Questions
                        .Where(x => (x.Subject.ToLower()
                        .Contains(request.Filter.ToLower()) || (x.User.FirstName.ToLower().Contains(usersName[0].ToLower()) && x.User.LastName.ToLower().Contains(usersName[1].ToLower()))) && (!statuses.Any() || statuses.Contains(x.Status)));
                }
                else
                {
                    filtered = _myheartContext.Questions
                        .Where(x => (x.Subject.ToLower()
                        .Contains(request.Filter.ToLower())) && (!statuses.Any() || statuses.Contains(x.Status)));
                }

                questions = await filtered
                    .Skip((request.Page - 1) * request.PageSize)
                    .ApplyOrdering(request.ColumnOrders)
                    .Take(request.PageSize).Select(q => new QuestionListDTO
                    {
                        Id = q.Id,
                        Subject = q.Subject,
                        Status = q.Status,
                        CreationDate = q.CreationDate.ToString("dd-MM-yyyy"),
                        UserId = q.UserId,
                        Name = q.User.FirstName + " " + q.User.LastName,
                        LastUpdateDate = q.LastUpdateDate,
                        LastUpdateUser = q.LastUpdateUser
                    }).ToListAsync();

                totalCount = await filtered
                    .CountAsync();
            }
            else
            {
                questions = await _myheartContext.Questions
                    .Skip((request.Page - 1) * request.PageSize)
                    .ApplyOrdering(request.ColumnOrders)
                    .Take(request.PageSize).Select(q => new QuestionListDTO
                    {
                        Id = q.Id,
                        Subject = q.Subject,
                        Status = q.Status,
                        CreationDate = q.CreationDate.ToString("dd-MM-yyyy"),
                        UserId = q.UserId,
                        Name = q.User.FirstName + " " + q.User.LastName,
                        LastUpdateDate = q.LastUpdateDate,
                        LastUpdateUser = q.LastUpdateUser
                    }).ToListAsync();

                totalCount = await _myheartContext.Questions
                    .CountAsync();
            }

            return new DataTableResponse<QuestionListDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = questions
            };
        }

        public async Task<DataTableResponse<QuestionCommentDTO>> GetLastCommentsOfQuestion(int questionId, int page, int pageSize)
        {
            var responses = await _myheartContext.QuestionComments.Where(x => x.QuestionId == questionId)
                .OrderByDescending(x => x.CreationDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new QuestionCommentDTO
                {
                    Id = x.Id,
                    QuestionId = x.QuestionId,
                    SenderId = x.UserId,
                    Text = x.Text,
                    CreationDate = x.CreationDate,
                    LastUpdateDate = x.LastUpdateDate,
                    LastUpdateUser = x.LastUpdateUser
                })
                .OrderBy(x => x.CreationDate)
                .ToListAsync();

            var count = await _myheartContext.QuestionComments
                .Where(x => x.QuestionId == questionId)
                .CountAsync();

            return new DataTableResponse<QuestionCommentDTO>
            {
                Data = responses,
                Page = page,
                PageSize = pageSize,
                TotalCount = count
            };
        }

        public async Task<VideoRoomDTO> GetVideoRoomDetails(int questionId)
        {
            if ((await _myheartContext.Questions.FindAsync(questionId)) == null)
            {
                return null;
            }

            var room = await _myheartContext.VideoRooms
                .Where(x => x.QuestionID == questionId)
                .FirstOrDefaultAsync();

            if (room == null)
            {
                Guid roomGuid;
                var unique = true;
                do
                {
                    roomGuid = Guid.NewGuid();
                    unique = await _myheartContext.VideoRooms.Where(x => x.RoomId == roomGuid).CountAsync() == 0;
                } while (!unique);

                room = new VideoRoom
                {
                    QuestionID = questionId,
                    RoomId = roomGuid,
                    Password = DoctorService.RandomString(16),
                    LastUpdateUser = "SYSTEM_GENERATED"
                };

                await _myheartContext.VideoRooms.AddAsync(room);
                await _myheartContext.SaveChangesAsync();
            }

            return _mapper.Map<VideoRoomDTO>(room);
        }

        public async Task<IEnumerable<QuestionCommentDTO>> GetClosedQuestionCommentsByDoctorIds(int[] doctorIds)
        {
            var comments = await _myheartContext.QuestionComments
                .Include(c => c.Question)
                .Where(c => c.Question.Status == QuestionStatus.Closed)
                .Where(c => doctorIds.Contains(c.UserId))
                .ToListAsync();

            return comments.Select(_mapper.Map<QuestionCommentDTO>);
        }



        public async Task<bool> DeleteQuestion(int id)
        {
            var question = await _myheartContext.Questions.FindAsync(id);

            if (question == null)
            {
                return false;
            }

            _myheartContext.Remove(question);

            await _myheartContext.SaveChangesAsync();

            return true;
        }
    }


}
