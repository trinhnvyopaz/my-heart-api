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
    public class MedicalFacilitiesController : ControllerBase
    {
        private readonly IMedicalFacilitieService _profInfoService;
        private readonly IAuthenticationService _authenticationService;
        private readonly MyHeartContext _context;
        private readonly IUserService _userService;

        public MedicalFacilitiesController(IMedicalFacilitieService profInfoService, IAuthenticationService authenticationService, MyHeartContext context, IUserService userService)
        {
            _authenticationService = authenticationService;
            _profInfoService = profInfoService;
            _context = context;
            _userService = userService;
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalFacilitiesDTO>>> GetAllMedicalFacilitiesAsync()
        {
            var medicalFacilities = await _profInfoService.MedicalFacilitiesListAsync();

            return Ok(medicalFacilities);
        }

        [HttpPost("getDataTable")]
        public async Task<IActionResult> GetMedicalFacilitiesTable(DataTableRequest request) {
            var result = await _profInfoService.GetMedicalFacilitiesTable(request);

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

        [HttpGet("{MedicalFacilitiesId}")]
        public async Task<ActionResult<MedicalFacilitiesDTO>> GetMedicalFacilitie(int medicalFacilitiesId)
        {
            var therapyNews = await _profInfoService.MedicalFacilitiesAsync(medicalFacilitiesId);

            return Ok(therapyNews);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPut]
        public async Task<ActionResult<MedicalFacilitiesDTO>> UpdateMedicalFacilities(MedicalFacilitiesDTO medicalFacilities)
        {
            if (medicalFacilities == null)
            {
                return BadRequest();
            }

            var dbtherapyNews = await _profInfoService.UpdateMedicalFacilities(medicalFacilities, await GetUserName());

            if (dbtherapyNews == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPost]
        public async Task<ActionResult<MedicalFacilitiesDTO>> CreateMedicalFacilities(MedicalFacilitiesDTO medicalFacilities)
        {
            var dbtherapyNews = await _profInfoService.AddMedicalFacilitiesAsync(medicalFacilities, await GetUserName());

            return Ok(dbtherapyNews);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpDelete("{medicalFacilitiesId}")]
        public async Task<ActionResult<MedicalFacilitiesDTO>> DeleteMedicalFacilities(int medicalFacilitiesId)
        {
            if (medicalFacilitiesId <= 0)
            {
                return BadRequest();
            }

            var symptom = await _profInfoService.DeleteMedicalFacilities(medicalFacilitiesId);

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