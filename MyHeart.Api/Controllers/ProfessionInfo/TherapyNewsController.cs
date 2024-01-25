using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHeart.Core.Helpers;
using MyHeart.Services.Interfaces;
using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHeart.Data;
using Microsoft.EntityFrameworkCore;
using MyHeart.Api.Util;
using MyHeart.DTO.User;
using System.Security.Claims;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TherapyNewsController : ControllerBase
    {
        private readonly ITherapyNewsService _profInfoService;
        private readonly IAuthenticationService _authenticationService;
        private readonly MyHeartContext _context;
        private readonly IUserService _userService;

        public TherapyNewsController(ITherapyNewsService profInfoService, IAuthenticationService authenticationService, MyHeartContext context, IUserService userService)
        {
            _authenticationService = authenticationService;
            _profInfoService = profInfoService;
            _context = context;
            _userService = userService;
        }

        [Authorize(Policy = Policies.MinPatient)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TherapyNewsDTO>>> GetAllTherapyNewsAsync()
        {
            var therapyNews = await _profInfoService.TherapyNewsListAsync();

            return Ok(therapyNews);
        }

        [Authorize(Policy = Policies.MinPatient)]
        [HttpPost("getDataTable")]
        public async Task<IActionResult> GetTherapyNewsTable(DataTableRequest request) {
            var result = await _profInfoService.GetTherapyNewsTable(request);

            return Ok(result);
        }

        [Authorize(Policy = Policies.MinPatient)]
        [HttpGet("{TherapyNewsId}")]
        public async Task<ActionResult<TherapyNewsDTO>> GetTherapyNew(int therapyNewsId)
        {
            var therapyNews = await _profInfoService.TherapyNewAsync(therapyNewsId);

            return Ok(therapyNews);
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpPut]
        public async Task<ActionResult<TherapyNewsDTO>> UpdateTherapyNews(TherapyNewsDTO therapyNews)
        {
            if (therapyNews == null)
            {
                return BadRequest();
            }

            var dbtherapyNews = await _profInfoService.UpdateTherapyNews(therapyNews, await GetUserName());

            if (dbtherapyNews == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpPost]
        public async Task<ActionResult<TherapyNewsDTO>> CreateTherapyNews(TherapyNewsDTO therapyNews)
        {
            var errors = await _profInfoService.ValidateTherapyNews(therapyNews);

            if (errors.Count() > 0)
            {
                return BadRequest(new ErrorResponse { Errors = errors, Title = "one or more validation errors occurred" });
            }
            var dbtherapyNews = await _profInfoService.AddTherapyNewsAsync(therapyNews, await GetUserName());

            return Ok(dbtherapyNews);
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpDelete("{TherapyNewsId}")]
        public async Task<ActionResult<TherapyNewsDTO>> DeleteTherapyNews(int therapyNewsId)
        {
            if (therapyNewsId <= 0)
            {
                return BadRequest();
            }

            var symptom = await _profInfoService.DeleteTherapyNews(therapyNewsId);

            if (symptom == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [Authorize(Policy = Policies.MinPatient)]
        [HttpGet("getSubscriptionPreferences")]
        public IActionResult GetSubscriptionPreferences()
        {
            var result = _profInfoService.GetSubscriptionPreferences();

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpGet("getSubscriptionPreferences/{id}")]
        public IActionResult GetSubscriptionPreferences(int id) {
            var result = _profInfoService.GetSubscriptionPreferences(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [Authorize(Policy = Policies.MinPatient)]
        [HttpPost("updateSubscriptionPreferences")]
        public async Task<IActionResult> UpdateSubscriptionPreferences(TherapyNewsSubscriptionSettingsDto model)
        {
            var user = await _userService.CurrentUserAsync();
            if (model.UserId != user.Id && user.UserType == UType.Patient) {
                return Unauthorized();
            }

            var result = _profInfoService.UpdateSubscriptionPreferences(model, await GetUserName());

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        private async Task<string> GetUserName() {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.GetUserDetail(userId);
            return $"{user?.FirstName} {user?.LastName}";
        }
    }

}