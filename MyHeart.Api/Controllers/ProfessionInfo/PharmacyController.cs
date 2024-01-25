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
    public class PharmacyController : ControllerBase
    {
        private readonly IPharmacyService _profInfoService;
        private readonly IAuthenticationService _authenticationService;
        private readonly MyHeartContext _context;
        private readonly IUserService _userService;

        public PharmacyController(IPharmacyService profInfoService, IAuthenticationService authenticationService, MyHeartContext context, IUserService userService)
        {
            _authenticationService = authenticationService;
            _profInfoService = profInfoService;
            _context = context;
            _userService = userService;
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PharmacyDTO>>> GetAllPharmacyAsync()
        {
            var pharmacy = await _profInfoService.PharmacyListAsync();

            return Ok(pharmacy);
        }

        [HttpPost("getDataTable")]
        public async Task<IActionResult> GetPharmaciesTable(DataTableRequest request)
        {
            var result = await _profInfoService.GetPharmaciesTable(request);

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

        [HttpGet("{PharmacyId}")]
        public async Task<ActionResult<PharmacyDTO>> GetPharmacy(int pharmacyId)
        {
            var pharmacy = await _profInfoService.PharmacyAsync(pharmacyId);

            return Ok(pharmacy);
        }

        //[Route("/namePart/{nameReg}")]
        [HttpGet("namePart/{nameReg}")]
        //[HttpGet]
        public async Task<ActionResult<IEnumerable<PharmacyDTO>>> GetPharmacies(string nameReg)
        {
            var selectedPharma = await _profInfoService.PharmacyAsync(nameReg);

            return Ok(selectedPharma);
            //return Ok(null);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPut]
        public async Task<ActionResult<PharmacyDTO>> UpdatePharmacy(PharmacyDTO pharmacy)
        {
            if (pharmacy == null)
            {
                return BadRequest();
            }

            var dbpharmacy = await _profInfoService.UpdatePharmacy(pharmacy, await GetUserName());

            if (dbpharmacy == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPost]
        public async Task<ActionResult<PharmacyDTO>> CreatePharmacy(PharmacyDTO pharmacy)
        {
            var dbmedicamentGroup = await _profInfoService.AddPharmacyAsync(pharmacy, await GetUserName());

            return Ok(dbmedicamentGroup);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpDelete("{PharmacyId}")]
        public async Task<ActionResult<PharmacyDTO>> DeletePharmacy(int pharmacyId)
        {
            if (pharmacyId <= 0)
            {
                return BadRequest();
            }

            var pharmacy = await _profInfoService.DeletePharmacy(pharmacyId);

            if (pharmacy == null)
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