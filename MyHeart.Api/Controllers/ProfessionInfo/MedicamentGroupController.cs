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
using MyHeart.DTO.ProfessionInformation;
using MyHeart.Api.Util;
using System.Security.Claims;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicamentGroupController : ControllerBase
    {
        private readonly IMedicamentGroupService _profInfoService;
        private readonly IAuthenticationService _authenticationService;
        private readonly MyHeartContext _context;
        private readonly IUserService _userService;

        public MedicamentGroupController(IMedicamentGroupService profInfoService, IAuthenticationService authenticationService, MyHeartContext context, IUserService userService)
        {
            _authenticationService = authenticationService;
            _profInfoService = profInfoService;
            _context = context;
            _userService = userService;
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicamentGroupDTO>>> GetAllMedicamentGroupAsync()
        {
            var medicamentGroup = await _profInfoService.MedicamentGroupListAsync();

            return Ok(medicamentGroup);
        }

        [HttpPost("getDataTable")]
        public async Task<IActionResult> GetMedicamentGroupsTable(DataTableRequest request) {
            var result = await _profInfoService.GetMedicamentGroupsTable(request);

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

        [HttpGet("{MedicamentGroupId}")]
        public async Task<ActionResult<MedicamentGroupDTO>> GetMedicamentGroup(int medicamentGroupId)
        {
            var medicamentGroup = await _profInfoService.MedicamentGroupAsync(medicamentGroupId);

            return Ok(medicamentGroup);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPut]
        public async Task<ActionResult<MedicamentGroupDTO>> UpdateMedicamentGroup(MedicamentGroupDTO medicamentGroup)
        {
            if (medicamentGroup == null)
            {
                return BadRequest();
            }

            var dbmedicamentGroup = await _profInfoService.UpdateMedicamentGroup(medicamentGroup, await GetUserName());

            if (dbmedicamentGroup == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPost]
        public async Task<ActionResult<MedicamentGroupDTO>> CreateMedicamentGroup(MedicamentGroupDTO medicamentGroup)
        {
            var dbmedicamentGroup = await _profInfoService.AddMedicamentGroupAsync(medicamentGroup, await GetUserName());

            return Ok(dbmedicamentGroup);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpDelete("{MedicamentGroupId}")]
        public async Task<ActionResult<MedicamentGroupDTO>> DeleteMedicamentGroup(int medicamentGroupId)
        {
            if (medicamentGroupId <= 0)
            {
                return BadRequest();
            }

            var medicamentGroup = await _profInfoService.DeleteMedicamentGroup(medicamentGroupId);

            if (medicamentGroup == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("getExcludedPharmacies/{medicamentGroupId}")]
        public async Task<IActionResult> GetExcludedPharmacies(int medicamentGroupId)
        {
            if (medicamentGroupId < 1)
                return BadRequest();

            var result = await _profInfoService.GetExcludedPharmacies(medicamentGroupId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("getDiscardedPharmacies")]
        public async Task<IActionResult> GetDiscardedPharmacies(DiscardedPharmaciesRequest request)
        {
            if (request == null || request.Group < 1)
                return BadRequest();

            var result = await _profInfoService.GetDiscardedPharmacies(request);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPost("togglePharmacyDiscard")]
        public async Task<IActionResult> TogglePharmacyDiscard([FromBody]MedicamentGroup_Pharmacy_ByAtc pharmacy)
        {
            if (pharmacy == null)
                return BadRequest();

            var result = await _profInfoService.TogglePharmacyDiscard(pharmacy, await GetUserName());

            return Ok(result);
        }

        private async Task<string> GetUserName() {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.GetUserDetail(userId);
            return $"{user?.FirstName} {user?.LastName}";
        }
    }

}