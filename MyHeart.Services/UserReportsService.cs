using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.DTO.User;
using MyHeart.DTO;
using MyHeart.Services.Interfaces.Client;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services
{
    public class UserReportsService : IUserReportsService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MyHeartContext _myheartContext;


        public UserReportsService(IHttpContextAccessor httpContextAccessor, MyHeartContext gradologyContext, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _myheartContext = gradologyContext;
            _mapper = mapper;
        }

        private async Task<User> GetCurrentUser()
        {
            if (_httpContextAccessor.HttpContext.User == null)
                return null;

            var userIdString = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdString, out int userId))
                return null;

            var user = await _myheartContext.Users.FindAsync(userId);

            if (user == null)
                return null;

            return user;
        }

        private async Task<User> GetUserById(int id)
        {
            return await _myheartContext.Users.FindAsync(id);
        }

        public async Task<IEnumerable<UserReportDTO>> GetReports()
        {
            var user = await GetCurrentUser();

            return await GetReports(user);
        }

         public async Task<DataTableResponse<UserReportDTO>> GetAllReports(DataTableRequest request)
        {
            int total = 0;
            IQueryable<UserReport> query = _myheartContext.UserReport;
            if (request.Filter != null)
            {
                query = query.Where(u => u.Description.Contains(request.Filter));
            }
            total = await query.CountAsync();
            var result = await query
                .OrderByDescending(x => x.CreationDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new DataTableResponse<UserReportDTO> {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = total,
                Data = _mapper.Map<IEnumerable<UserReport>, IList<UserReportDTO>>(result)
            };
        }

        public async Task<IEnumerable<UserReportDTO>> GetReports(int id)
        {
            var user = await GetUserById(id);

            return await GetReports(user);
        }

        private async Task<IEnumerable<UserReportDTO>> GetReports(User user)
        {
            if (user == null)
                return null;

            var result = await _myheartContext.UserReport
                .Include(x => x.UserReportFiles)
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.CreationDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserReport>, IEnumerable<UserReportDTO>>(result);
        }

        public async Task<UserReportDTO> UpdateReport(UserReportDTO model, string userName)
        {
            var report = await _myheartContext.UserReport
                .Include(u => u.UserReportFiles)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (report == null)
                return null;


            report.ReportType = (ReportType)model.ReportType;
            report.Description = model.Description;
            report.Title = model.Title;
            report.LastUpdateUser = userName;
            report.LastUpdateDate = DateTime.Now;

            foreach (var file in model.Files)
            {
                if (file.Id == 0)
                {
                    report.UserReportFiles.Add(new UserReportFile
                    {
                        Extension = file.Extension,
                        Content = file.Content,
                        Name = file.Name,
                        MimeType = file.MimeType,
                        UserReportId = report.Id,
                        LastUpdateUser = userName,
                        LastUpdateDate = DateTime.Now,                        
                    });
                }
            }

            await _myheartContext.SaveChangesAsync();

            return _mapper.Map<UserReportDTO>(report);
        }

        public async Task<UserReportDTO> SaveReport(UserReportDTO model, string userName)
        {
            var files = new List<UserReportFile>();

            var report = new UserReport
            {
                UserId = model.UserId,
                Title = model.Title,
                ReportType = (ReportType)model.ReportType,
                Description = model.Description,
                CreationDate = DateTime.Now,
                LastUpdate = DateTime.Now,
                LastUpdateUser = userName,
                UserReportFiles = model.Files.Select(f => new UserReportFile
                {
                    Name = f.Name,
                    Extension = f.Extension,
                    Content = f.Content,
                    MimeType = f.MimeType,
                    LastUpdateDate = DateTime.Now,
                    LastUpdateUser = userName
                }).ToList()
            };

            _myheartContext.UserReport
                .Add(report);

            await _myheartContext.SaveChangesAsync();

            return _mapper.Map<UserReport, UserReportDTO>(report);
        }

        public async Task<bool> DeleteUserReport(int id)
        {
            var files = await _myheartContext.UserReportFile
                .Include(x => x.UserReport)
                .Where(x => x.UserReportId == id)
                .ToListAsync();

            _myheartContext.UserReportFile
                .RemoveRange(files);

            await _myheartContext.SaveChangesAsync();

            var report = await _myheartContext.UserReport
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (report == null)
                return false;

            _myheartContext.UserReport
                .Remove(report);

            await _myheartContext.SaveChangesAsync();

            return true;
        }

        public async Task<UserReportFileDTO> GetUserReportFile(int reportFileId)
        {
            var currentUser = await GetCurrentUser();

            //if (currentUser == null)
            //    return null;

            var reportFile = await _myheartContext.UserReportFile
                .Include(x => x.UserReport)
                .Where(x => x.Id == reportFileId)
                .FirstOrDefaultAsync();

            //if (reportFile.UserReport.UserId != currentUser.Id)
            //    return null;

            return _mapper.Map<UserReportFile, UserReportFileDTO>(reportFile);
        }

        public async Task<bool> DoesReportBelongToUser(int userId, int reportId)
        {
            var report = _myheartContext.UserReport.Find(reportId);

            if (report == null)
            {
                return false;
            }

            return report.UserId == userId;
        }

        public async Task<bool> DoesReportFileBelongToUser(int userId, int fileId)
        {
            var file = await _myheartContext.UserReportFile
                .Include(x => x.UserReport)
                .Where(x => x.Id == fileId)
                .FirstOrDefaultAsync();

            if (file == null)
            {
                return false;
            }

            return file.UserReport.UserId == userId;
        }
    }
}
