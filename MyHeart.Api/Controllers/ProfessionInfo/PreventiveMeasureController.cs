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
    public class PreventiveMeasureController : ControllerBase
    {
        private readonly IPreventiveMeasureService _profInfoService;
        private readonly IAuthenticationService _authenticationService;
        private readonly MyHeartContext _context;
        private readonly IUserService _userService;

        public PreventiveMeasureController(IPreventiveMeasureService profInfoService, IAuthenticationService authenticationService, MyHeartContext context, IUserService userService)
        {
            _authenticationService = authenticationService;
            _profInfoService = profInfoService;
            _context = context;
            _userService = userService;
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PreventiveMeasuresDTO>>> GetAllPreventiveMeasuresAsync()
        {
            var preventiveMeasures = await _profInfoService.PreventiveMeasuresListAsync();

            return Ok(preventiveMeasures);
        }

        [HttpPost("getDataTable")]
        public async Task<IActionResult> GetPreventiveMeasuresTable(DataTableRequest request) {
            var result = await _profInfoService.GetPreventiveMeasuresTable(request);

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

        [HttpGet("{PreventiveMeasuresId}")]
        public async Task<ActionResult<PreventiveMeasuresDTO>> GetPreventiveMeasures(int preventiveMeasuresId)
        {
            var preventiveMeasures = await _profInfoService.PreventiveMeasuresAsync(preventiveMeasuresId);

            return Ok(preventiveMeasures);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPut]
        public async Task<ActionResult<PreventiveMeasuresDTO>> UpdatePreventiveMeasures(PreventiveMeasuresDTO preventiveMeasures)
        {
            if (preventiveMeasures == null)
            {
                return BadRequest();
            }

            var dbpreventiveMeasures = await _profInfoService.UpdatePreventiveMeasures(preventiveMeasures, await GetUserName());

            if (dbpreventiveMeasures == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPost]
        public async Task<ActionResult<PreventiveMeasuresDTO>> CreatePreventiveMeasures(PreventiveMeasuresDTO preventiveMeasures)
        {
            var dbtherapyNews = await _profInfoService.AddPreventiveMeasuresAsync(preventiveMeasures, await GetUserName());

            return Ok(dbtherapyNews);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpDelete("{PreventiveMeasuresId}")]
        public async Task<ActionResult<PreventiveMeasuresDTO>> DeletePreventiveMeasures(int preventiveMeasuresId)
        {
            if (preventiveMeasuresId <= 0)
            {
                return BadRequest();
            }

            var symptom = await _profInfoService.DeletePreventiveMeasures(preventiveMeasuresId);

            if (symptom == null)
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