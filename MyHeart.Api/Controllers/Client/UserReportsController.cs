using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using MyHeart.Api.Util;
using MyHeart.DTO;
using MyHeart.DTO.User;
using MyHeart.Services;
using MyHeart.Services.Interfaces;
using MyHeart.Services.Interfaces.Client;
using MyHeart.Services.Interfaces.ProfessionInfo;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserReportsController : Controller
    {
        private readonly IUserReportsService _userReportsService;
        private readonly IUserService _userService;

        public UserReportsController(
            IUserReportsService userReportsService,
            IUserService userService
        )
        {
            _userReportsService = userReportsService;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpGet("getUserReports")]
        public async Task<IActionResult> GetUserReports()
        {
            var result = await _userReportsService.GetReports();

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("getAllUserReports")]
        public async Task<IActionResult> GetAllUserReports(DataTableRequest request)
        {
            var result = await _userReportsService.GetAllReports(request);

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpGet("getUserReports/{id}")]
        public async Task<IActionResult> GetUserReports(int id)
        {
            var result = await _userReportsService.GetReports(id);

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpPost("saveUserReport")]
        public async Task<IActionResult> SaveUserReport([FromBody] UserReportDTO model)
        {
            if (model == null)
                return BadRequest();

            var user = await _userService.CurrentUserAsync();
            if (user.Id != model.UserId && user.UserType == UType.Patient)
            {
                return Unauthorized();
            }

            var result = await _userReportsService.SaveReport(model, await GetUserName());

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpPost("saveUserReportFile")]
        public async Task<IActionResult> SaveUserReport(List<IFormFile> files)
        {
            var user = await _userService.CurrentUserAsync();

            var model = new UserReportDTO
            {
                UserId = user.Id,
                Title = "Analyse example",
                Description = "Analyse example",
                ReportType = ReportTypeDTO.AmbulanceReport,
                Files = new List<UserReportFileDTO>(),
            };
            byte[] fileBytes;
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();
                        // act on the Base64 data
                    }
                    var userReportFile = new UserReportFileDTO
                    {
                        Name = file.FileName,
                        Extension = Path.GetExtension(file.FileName),
                        Content = fileBytes,
                        MimeType = file.ContentType
                    };
                    model.Files.Add(userReportFile);
                }
            }

            var result = await _userReportsService.SaveReport(model, await GetUserName());

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpPost("updateUserReport")]
        public async Task<IActionResult> UpdateUserReport(UserReportDTO model)
        {
            if (model == null)
                return BadRequest();

            var user = await _userService.CurrentUserAsync();
            if (user.Id != model.UserId && user.UserType == UType.Patient)
            {
                return Unauthorized();
            }

            var result = await _userReportsService.UpdateReport(model, await GetUserName());

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpDelete("deleteUserReport/{id}")]
        public async Task<IActionResult> DeleteUserReport(int id)
        {
            var user = await _userService.CurrentUserAsync();
            if (
                !(await _userReportsService.DoesReportBelongToUser(user.Id, id))
                && user.UserType == UType.Patient
            )
            {
                return Unauthorized();
            }

            if (id < 1)
                return BadRequest();

            if (await _userReportsService.DeleteUserReport(id))
                return Ok();

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet("downloadReportFile/{reportFileId}")]
        public async Task<IActionResult> DownloadReportFile(int reportFileId)
        {
            var user = await _userService.CurrentUserAsync();
            if (
                !(await _userReportsService.DoesReportFileBelongToUser(user.Id, reportFileId))
                && user.UserType == UType.Patient
            )
            {
                return Unauthorized();
            }

            var result = await _userReportsService.GetUserReportFile(reportFileId);

            if (result == null)
                return BadRequest();

            Response.Headers.Add("Content-Description", "File Transfer");
            Response.Headers.Add(
                "Content-Disposition",
                "attachment; filename=\"" + result.Name + "\""
            );
            return File(result.Content, result.MimeType);
        }

        private async Task<string> GetUserName()
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.GetUserDetail(userId);
            return $"{user?.FirstName} {user?.LastName}";
        }
    }
}
