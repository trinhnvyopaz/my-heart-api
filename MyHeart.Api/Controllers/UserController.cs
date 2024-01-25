using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHeart.Core.Helpers;
using MyHeart.Services.Interfaces;
using MyHeart.DTO;
using MyHeart.DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHeart.Data;
using Microsoft.EntityFrameworkCore;
using MyHeart.Data.Models;
using MyHeart.Api.Util;
using MyHeart.DTO.Questions;
using System.Text.RegularExpressions;
using System.Security.Claims;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly MyHeartContext _context;
        private readonly IQuestionService _questionService;
        private readonly IMFAService _mfaService;
        private static readonly Regex _regex = new Regex(@"\d{9}", RegexOptions.Compiled);

        public UserController(
            IUserService userService,
            IAuthenticationService authenticationService,
            MyHeartContext context,
            IQuestionService questionService,
            IMFAService mfaService
        )
        {
            _authenticationService = authenticationService;
            _userService = userService;
            _context = context;
            _questionService = questionService;
            _mfaService = mfaService;
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpGet("list/{userTypeId}")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsersAsync(int userTypeId)
        {
            var users = await _userService.ListAsync((UType)userTypeId);

            return Ok(users);
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpPost("getDataTable")]
        public async Task<ActionResult<DataTableResponse<UserDTO>>> GetUserDataTable(
            GroupedDataTableRequest request
        )
        {
            var result = await _userService.GetUsersTable(request);

            return Ok(result);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult<UserDTO>> DeleteUser(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest();
            }

            var currentUser = await _userService.CurrentUserAsync();
            if (currentUser.Id != userId && currentUser.UserType == UType.Patient)
            {
                return Unauthorized();
            }

            var user = await _userService.DeleteUser(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPatch("ChangeRole")]
        public async Task<ActionResult<UserDTO>> ChangeUserRole([FromBody] UserDTO userDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userDTO.Id);

            var currentUser = await _userService.CurrentUserAsync();

            if (currentUser.Id == user.Id)
            {
                return BadRequest();
            }
            else
            {
                var selectedUserType = userDTO.UserType;
                user.UserType = selectedUserType;
                user.LastUpdateUser = await GetUserName();
                var userRole = await _context.Roles.FirstOrDefaultAsync(
                    u => u.UserId == userDTO.Id && u.RoleType == userDTO.UserType
                );

                if (userRole != null)
                {
                    userRole.RoleType = selectedUserType;
                    userRole.LastUpdateUser = await GetUserName();

                    var roleDataManager = await _context.Roles.FirstOrDefaultAsync(
                        u => u.UserId == userDTO.Id && u.RoleType == UType.DataManager
                    );

                    if (selectedUserType == UType.Doctor && roleDataManager == null)
                    {
                        var role = new Role() { UserId = user.Id, RoleType = UType.DataManager, };
                        _context.Add(role);
                    }
                }

                _context.SaveChanges();

                return Ok();
            }
        }

        [AllowAnonymous]
        [HttpGet("Current")]
        public async Task<ActionResult<UserDTO>> GetCurrentAsync()
        {
            var user = await _userService.CurrentUserAsync();

            return Ok(user);
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _userService.UserByIdAsync(id);

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerViewModel)
        {
            var errors = await _userService.Validate(registerViewModel);

            if (errors.Count() > 0)
            {
                return BadRequest(
                    new ErrorResponse
                    {
                        Errors = errors,
                        Title = "one or more validation errors occurred"
                    }
                );
            }
            var user = await _userService.RegisterUserAsync(registerViewModel);

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPatch("Activate/{token}")]
        public async Task<ActionResult> ActivateUser(string token)
        {
            if (!Guid.TryParse(token, out Guid guid))
            {
                return BadRequest();
            }

            var user = await _userService.ActivateUser(guid);

            if (user == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginDTO loginViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState.GetErrorMessages());

                var userId = await _userService.GetUserIdByEmail(loginViewModel.Email);
                if (userId == -1)
                {
                    return Unauthorized(
                        new ErrorResponse() { Title = "Email nebo heslo bylo zadáno špatně" }
                    );
                }

                var token = await _authenticationService.Authenticate(loginViewModel);
                // return Ok(token);
                if (token == null)
                    return Unauthorized(
                        new ErrorResponse() { Title = "Email nebo heslo bylo zadáno špatně" }
                    );

                if (!(await _userService.MFAConfirmedById(userId)))
                {
                    return Ok(new ConfirmNeededDTO() { Error = "MFACONFIRM", Id = userId });
                }

                if (!(await _userService.EmailConfirmedById(userId)))
                {
                    return Unauthorized(new ErrorResponse() { Title = "Email není ověřen" });
                }

                if (await _mfaService.DoesUserHaveActiveMFA(userId))
                {
                    return Ok("MFANEEDED");
                }
                else
                {
                    await _mfaService.SendMFASMS(userId);
                    return Ok("MFANEEDED");
                }

                //return Ok(token);
            }
            catch (Exception e)
            {
                if (e.Message == "WRONGMFA")
                {
                    return Unauthorized(new ErrorResponse() { Title = "MFA bylo zadáno špatně" });
                }
                else if (e.Message == "EMAILCONFIRM")
                {
                    return Unauthorized(new ErrorResponse() { Title = "Email není ověřen" });
                }
                else if (e.Message == "MFACONFIRM")
                {
                    var userId = await _userService.GetUserIdByEmail(loginViewModel.Email);
                    return Ok(new ConfirmNeededDTO() { Error = "MFACONFIRM", Id = userId });
                }
                else
                {
                    return Unauthorized(
                        new ErrorResponse() { Title = "Email nebo heslo bylo zadáno špatně" }
                    );
                }
            }
        }

        [AllowAnonymous]
        [HttpGet("validateFirstSMS/{id}/{code}")]
        public async Task<IActionResult> ValidateFirstSMS(int id, string code)
        {
            var user = await _userService.UserByIdAsync(id);

            if (user == null || await _userService.MFAConfirmedById(id))
            {
                return BadRequest("BADUSER");
            }

            if (await _mfaService.AlreadyValidated(user.Id))
            {
                return BadRequest("2FA již bylo zvalidováno");
            }

            var status = await _mfaService.ValidateCode(user.Id, code);
            if (!status)
            {
                return BadRequest("Nesprávný kód");
            }

            await _mfaService.FirstValidation(id);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("generateFirstSMS/{id}/{number}")]
        public async Task<IActionResult> GenerateFirstSMS(int id, string number)
        {
            var num = number.Replace(" ", "");
            if (!_regex.IsMatch(num))
            {
                return BadRequest("Nesprávné číslo, zadejte české číslo bez předpony (9 číslic)");
            }

            if (
                await _userService.MFAConfirmedById(id)
                || !await _userService.AddNumberToUser(id, number)
            )
            {
                return BadRequest("BADUSER");
            }

            if (!await _mfaService.SendMFASMS(id))
            {
                return BadRequest("SMS se nepodařilo odeslat");
            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("generateFirstMfa/{id}")]
        public async Task<ActionResult<MFAResponseDTO>> GenerateMfa(int id)
        {
            var user = await _userService.UserByIdAsync(id);

            if (user == null || await _userService.MFAConfirmedById(id))
            {
                return BadRequest("BADUSER");
            }

            var response = await _mfaService.CreateNewCodeForUser(user.Id);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("validateFirstMfa/{id}/{code}")]
        public async Task<ActionResult<string>> ValidateMFA(int id, string code)
        {
            var user = await _userService.UserByIdAsync(id);

            if (user == null || await _userService.MFAConfirmedById(id))
            {
                return BadRequest("BADUSER");
            }

            if (await _mfaService.AlreadyValidated(user.Id))
            {
                return BadRequest("2FA již bylo zvalidováno");
            }

            var status = await _mfaService.ValidateCode(user.Id, code);
            if (!status)
            {
                return BadRequest("Nesprávný kód");
            }

            await _mfaService.FirstValidation(id);

            return Ok();
        }

        [HttpGet("generateMfa")]
        public async Task<ActionResult<MFAResponseDTO>> GenerateMfa()
        {
            var user = await _userService.CurrentUserAsync();

            if (user == null)
            {
                return BadRequest("Pro vygenerování 2FA QR kódu musíte být přihlášen/a");
            }

            var response = await _mfaService.CreateNewCodeForUser(user.Id);

            return Ok(response);
        }

        [HttpGet("validateMfa/{code}")]
        public async Task<ActionResult<string>> ValidateMFA(string code)
        {
            var user = await _userService.CurrentUserAsync();

            if (user == null)
            {
                return BadRequest("Pro validaci 2FA QR kódu musíte být přihlášen/a");
            }

            if (await _mfaService.AlreadyValidated(user.Id))
            {
                return BadRequest("2FA již bylo zvalidováno");
            }

            var status = await _mfaService.ValidateCode(user.Id, code);
            if (!status)
            {
                return BadRequest("Nesprávný kód");
            }

            return Ok();
        }

        [HttpGet("removeMfa/{code}")]
        public async Task<ActionResult<string>> RemoveMfa(string code)
        {
            var user = await _userService.CurrentUserAsync();

            if (user == null)
            {
                return BadRequest("Pro validaci 2FA QR kódu musíte být přihlášen/a");
            }

            if (!await _mfaService.ValidateCode(user.Id, code))
            {
                return BadRequest("Nesprávný kód");
            }

            await _mfaService.Remove2FA(user.Id);
            return Ok();
        }

        [HttpGet("doesUserHaveMfa")]
        public async Task<ActionResult<string>> DoesUserHaveMfa()
        {
            var user = await _userService.CurrentUserAsync();

            if (user == null)
            {
                return BadRequest();
            }

            if (await _mfaService.DoesUserHaveActiveMFA(user.Id))
            {
                return Ok("active");
            }

            await _mfaService.Remove2FA(user.Id);
            return Ok("inactive");
        }

        [HttpGet("refresh")]
        public async Task<ActionResult<string>> Refresh()
        {
            var token = await _authenticationService.Refresh(await _userService.CurrentUserAsync());

            if (token == null)
            {
                return Unauthorized(new ErrorResponse() { Title = "Prihlašte se znovu." });
            }

            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("mfaLogin")]
        public async Task<ActionResult<LoginResponseDTO>> MfaLogin(MFALoginDTO loginViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState.GetErrorMessages());

                var userId = await _userService.GetUserIdByEmail(loginViewModel.Email);
                if (userId == -1)
                {
                    return Unauthorized(
                        new ErrorResponse() { Title = "Email nebo heslo bylo zadáno špatně" }
                    );
                }

                var token = await _authenticationService.Authenticate(loginViewModel);

                if (token == null)
                {
                    if (!(await _userService.MFAConfirmedById(userId)))
                    {
                        return Ok(new ConfirmNeededDTO() { Error = "MFACONFIRM", Id = userId });
                    }

                    if (!(await _userService.EmailConfirmedById(userId)))
                    {
                        return Unauthorized(new ErrorResponse() { Title = "Email není ověřen" });
                    }

                    if (await _mfaService.DoesUserHaveActiveMFA(userId))
                    {
                        return Unauthorized(
                            new ErrorResponse() { Title = "MFA bylo zadáno špatně" }
                        );
                    }
                    else
                    {
                        return Unauthorized(
                            new ErrorResponse() { Title = "Email nebo heslo bylo zadáno špatně" }
                        );
                    }
                }

                return Ok(token);
            }
            catch (Exception e)
            {
                if (e.Message == "WRONGMFA")
                {
                    return Unauthorized(new ErrorResponse() { Title = "MFA bylo zadáno špatně" });
                }
                else if (e.Message == "EMAILCONFIRM")
                {
                    return Unauthorized(new ErrorResponse() { Title = "Email není ověřen" });
                }
                else if (e.Message == "MFACONFIRM")
                {
                    var userId = await _userService.GetUserIdByEmail(loginViewModel.Email);
                    return Ok(new ConfirmNeededDTO() { Error = "MFACONFIRM", Id = userId });
                }
                else
                {
                    return Unauthorized(
                        new ErrorResponse() { Title = "Email nebo heslo bylo zadáno špatně" }
                    );
                }
            }
        }

        [AllowAnonymous]
        [HttpPost("mobileLogin")]
        public async Task<ActionResult<LoginResponseDTO>> MobileLogin(MobileLoginDTO loginViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState.GetErrorMessages());

                var token = await _authenticationService.MobileLogin(loginViewModel);

                return Ok(token);
            }
            catch (Exception e)
            {
                if (e.Message == "NULLUSER")
                {
                    return Unauthorized(new ErrorResponse() { Title = "Neznámý uživatel" });
                }
                else if (e.Message == "BADTOKEN")
                {
                    return Unauthorized(new ErrorResponse() { Title = "Nesptrávný token" });
                }
                else
                {
                    return Unauthorized(new ErrorResponse() { Title = "Přihlášení se nezdařilo" });
                }
            }
        }

        [HttpGet("mobileLogin")]
        public async Task<ActionResult<string>> MobileLogin()
        {
            return await _authenticationService.CreateMobileLoginForUser();
        }

        [AllowAnonymous]
        [HttpGet("loginByPhone")]
        public async Task<ActionResult<PhoneAuthDTO>> RequestLoginByPhone()
        {
            return await _authenticationService.RequestAuthorizeByPhone();
        }

        [AllowAnonymous]
        [HttpGet("loginByPhone/{id}/{guid}")]
        public async Task<ActionResult> CheckLoginByPhone(int id, Guid guid)
        {
            try
            {
                return Ok(await _authenticationService.CheckPhoneLogin(guid, id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("loginByPhone")]
        public async Task<ActionResult<string>> LoginByPhone(PhoneAuthResponseDTO response)
        {
            var user = await _userService.CurrentUserAsync();

            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(
                await _authenticationService.AuthorizeByPhone(
                    user,
                    response.Secret,
                    response.AuthId
                )
            );
        }

        [HttpGet("adminRemoveMfa/{id}")]
        [Authorize(Policy = Policies.MinAdmin)]
        public async Task<ActionResult> RemoveMfaForUser(int id)
        {
            if (!await _mfaService.DoesUserHaveActiveMFA(id))
            {
                return BadRequest("Uživatel nemá MFA");
            }

            var removed = await _mfaService.Remove2FA(id);

            if (!removed)
            {
                return BadRequest("Špatné Id uživatele");
            }

            await _userService.InvalidatePassword(id);

            return Ok();
        }

        [HttpPost("recover")]
        [AllowAnonymous]
        public async Task<ActionResult> SendRecoverPasswordLink([FromBody] string email)
        {
            var result = await _userService.SendRecoverPasswordLink(email);

            if (!result)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPatch("recoverpassword")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> RecoverPassword(
            [FromBody] PasswordRecoverDTO recoverDTO
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var user = await _userService.RecoverPassword(recoverDTO);

            if (user == null)
            {
                return BadRequest(
                    new ErrorResponse
                    {
                        Errors = new Dictionary<string, string>
                        {
                            { "Link", "Odkaz již není platný" }
                        }
                    }
                );
            }

            return Ok();
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpGet("Questions/{userId}")]
        public async Task<ActionResult<IEnumerable<QuestionListDTO>>> GetUserQuestions(int userId)
        {
            var questions = await _userService.GetUserQuestions(userId);

            return Ok(questions);
        }

        [HttpGet("detail/{userId}")]
        public async Task<ActionResult<UserDetailDTO>> GetUserDetail(int userId)
        {
            var currentUser = await _userService.CurrentUserAsync();
            if (currentUser.Id != userId && currentUser.UserType == UType.Patient)
            {
                return Unauthorized();
            }

            var detail = await _userService.GetUserDetail(userId);

            return Ok(detail);
        }

        [HttpGet("Questions/Comments/{questionId}")]
        public async Task<ActionResult<IEnumerable<QuestionListDTO>>> GetQuestionComments(
            int questionId
        )
        {
            var currentUser = await _userService.CurrentUserAsync();
            if (
                !(await _questionService.DoesQuestionBelongToUser(questionId, currentUser.Id))
                && currentUser.UserType == UType.Patient
            )
            {
                return Unauthorized();
            }

            var comments = await _userService.GetQuestionComments(questionId);

            return Ok(comments);
        }

        [HttpPatch("Update")]
        public async Task<ActionResult<UserDetailDTO>> UpdateUser(UserDetailDTO user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var currentUser = await _userService.CurrentUserAsync();
            if (currentUser.Id != user.Id && currentUser.UserType == UType.Patient)
            {
                return Unauthorized();
            }

            var dbuser = await _userService.UpdateUser(user, await GetUserName());

            if (dbuser == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("Reports/{userId}")]
        public async Task<ActionResult<IEnumerable<UserReportDTO>>> GetReports(int userId)
        {
            var currentUser = await _userService.CurrentUserAsync();
            if (currentUser == null)
            {
                return BadRequest();
            }

            if (currentUser.Id != userId && currentUser.UserType == UType.Patient)
            {
                return Unauthorized();
            }

            var reports = await _userService.GetUserReports(userId);

            return Ok(reports);
        }

        [HttpGet("Reports/{userId}/{reportId}")]
        public async Task<ActionResult<List<UserReportFileDTO>>> GetReportFiles(
            int userId,
            int reportId
        )
        {
            /*if (_context.Users.First(u => u.Id == userId) == null
                || _context.UserReport.First(ur => ur.Id == reportId) == null)
            {
                return BadRequest();
            }*/

            var currentUser = await _userService.CurrentUserAsync();
            if (currentUser == null)
            {
                return BadRequest();
            }

            if (currentUser.Id != userId && currentUser.UserType == UType.Patient)
            {
                return Unauthorized();
            }

            var reportFiles = await _userService.GetUserReportFiles(userId, reportId);
            var reportFilesList = reportFiles.ToList();

            if (reportFilesList.Count == 0)
            {
                return BadRequest();
            }

            return Ok(reportFilesList);
        }

        [HttpPost("Reports/Add/{userId}")]
        public async Task<ActionResult<bool>> AddReport(int userId, UserReportDTO newReport)
        {
            var currentUser = await _userService.CurrentUserAsync();
            if (newReport == null || _context.Users.First(u => u.Id == userId) == null)
            {
                return BadRequest();
            }

            if (currentUser.Id != userId && currentUser.UserType == UType.Patient)
            {
                return Unauthorized();
            }

            var report = await _userService.AddUserReport(userId, newReport, await GetUserName());

            if (report == null)
                return BadRequest();

            return Ok(report);
        }

        [HttpDelete("Reports/Delete/{userId}/{reportId}")]
        public async Task<ActionResult<bool>> DeleteReport(int userId, int reportId)
        {
            if (
                _context.Users.First(u => u.Id == userId) == null
                || _context.UserReport.First(ur => ur.Id == reportId) == null
            )
            {
                return BadRequest();
            }

            var currentUser = await _userService.CurrentUserAsync();
            if (currentUser.Id != userId && currentUser.UserType == UType.Patient)
            {
                return Unauthorized();
            }

            bool success = await _userService.DeleteUserReport(userId, reportId);

            return Ok(success);
        }

        private async Task<string> GetUserName()
        {
            var userId = Convert.ToInt32(
                User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value
            );
            var user = await _userService.GetUserDetail(userId);
            return $"{user?.FirstName} {user?.LastName}";
        }

        [HttpPatch("Reports/Update/{userId}/{reportId}")]
        public async Task<ActionResult<bool>> UpdateReport(
            int userId,
            int reportId,
            UserReportUpdateDTO reportUpdateDTO
        )
        {
            if (
                _context.Users.First(u => u.Id == userId) == null
                || _context.UserReport.First(ur => ur.Id == reportId) == null
            )
            {
                return BadRequest();
            }

            var currentUser = await _userService.CurrentUserAsync();
            if (currentUser.Id != userId && currentUser.UserType == UType.Patient)
            {
                return Unauthorized();
            }

            bool success = await _userService.UpdateReport(userId, reportId, reportUpdateDTO);

            return Ok(success);
        }

        [HttpGet("NotificationSettings")]
        public async Task<ActionResult<UserNotificationSettingsDTO>> GetUserNotificationSettings()
        {
            return Ok(await _userService.GetUserNotificationSettings());
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpGet("NotificationSettings/{userId}")]
        public async Task<ActionResult<UserNotificationSettingsDTO>> GetUserNotificationSettings(
            int userId
        )
        {
            return Ok(await _userService.GetUserNotificationSettings(userId));
        }

        [HttpPut("NotificationSettings")]
        public async Task<ActionResult<UserNotificationSettingsDTO>> UpdateUserNotificationSettings(
            UserNotificationSettingsDTO settings
        )
        {
            var user = await _userService.CurrentUserAsync();

            if (user == null || (user.Id != settings.UserID && user.UserType == UType.Patient))
            {
                return Unauthorized("You can only change your own settings");
            }

            return Ok(_userService.UpdateUserNotificationSettings(settings));
        }

        [HttpPatch("AddFmcToken")]
        public async Task<ActionResult<UserFmcTokenDTO>> AddFmcToken(UserFmcTokenDTO tokenDto)
        {
            var token = await _userService.AddFmcToken(tokenDto);
            return Ok(token);
        }
    }
}
