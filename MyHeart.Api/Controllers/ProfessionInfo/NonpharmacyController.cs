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
using System.Security.Claims;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NonpharmacyController : ControllerBase
    {
        private readonly INonpharmacyService _profInfoService;
        private readonly IAuthenticationService _authenticationService;
        private readonly MyHeartContext _context;
        private readonly IUserService _userService;

        public NonpharmacyController(INonpharmacyService profInfoService, IAuthenticationService authenticationService, MyHeartContext context, IUserService userService)
        {
            _authenticationService = authenticationService;
            _profInfoService = profInfoService;
            _context = context;
            _userService = userService;
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NonpharmacyDTO>>> GetAllNonpharmacyAsync()
        {
            var nonpharmacy = await _profInfoService.NonpharmacyListAsync();

            return Ok(nonpharmacy);
        }

        [HttpPost("getDataTable")]
        public async Task<IActionResult> GetNonpharmacyTable(DataTableRequest request) {
            var result = await _profInfoService.GetNonpharmacyTable(request);

            return Ok(result);
        }

        [HttpPost("getPagedResource")]
        public async Task<IActionResult> GetPagedResource(SortedPagedSourceRequest request) {
            var result = await _profInfoService.GetPagedResource(request);

            return Ok(result);
        }

        [HttpPost("getByIds")]
        public async Task<IActionResult> GetByIds(IEnumerable<int> ids) {
            var result = await _profInfoService.GetByIds(ids);

            return Ok(result);
        }

        [HttpGet("{NonpharmacyId}")]
        public async Task<ActionResult<NonpharmacyDTO>> GetNonpharmacy(int nonpharmacyId)
        {
            var nonpharmacy = await _profInfoService.NonpharmacyAsync(nonpharmacyId);

            return Ok(nonpharmacy);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPut]
        public async Task<ActionResult<NonpharmacyDTO>> UpdateNonpharmacy(NonpharmacyDTO nonpharmacy)
        {
            if (nonpharmacy == null)
            {
                return BadRequest();
            }

            var dbnonpharmacy = await _profInfoService.UpdateNonpharmacy(nonpharmacy, await GetUserName());

            if (dbnonpharmacy == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPost]
        public async Task<ActionResult<NonpharmacyDTO>> CreateNonpharmacy(NonpharmacyDTO nonpharmacy)
        {
            var dbnonpharmacy = await _profInfoService.AddNonpharmacyAsync(nonpharmacy, await GetUserName());

            return Ok(dbnonpharmacy);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpDelete("{NonpharmacyId}")]
        public async Task<ActionResult<NonpharmacyDTO>> DeleteNonpharmacy(int nonpharmacyId)
        {
            if (nonpharmacyId <= 0)
            {
                return BadRequest();
            }

            var nonpharmacy = await _profInfoService.DeleteNonpharmacy(nonpharmacyId);

            if (nonpharmacy == null)
            {
                return NotFound();
            }

            return Ok();
        }

        private async Task<string> GetUserName() {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.GetUserDetail(userId);
            return $"{user?.FirstName} {user?.LastName}";
        }

    }

}