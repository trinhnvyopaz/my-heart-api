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
using System.Text;
using MyHeart.Api.Util;
using System.Security.Claims;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DiseaseController : ControllerBase
    {
        private readonly IDiseaseService _profInfoService;
        private readonly IAuthenticationService _authenticationService;
        private readonly MyHeartContext _context;
        private readonly IUserService _userService;

        public DiseaseController(IDiseaseService profInfoService, IAuthenticationService authenticationService, MyHeartContext context, IUserService userService)
        {
            _authenticationService = authenticationService;
            _profInfoService = profInfoService;
            _context = context;
            _userService = userService;
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiseaseDTO>>> GetAllDiseaseAsync()
        {
            var disease = await _profInfoService.DiseaseListAsync();

            return Ok(disease);
        }

        [HttpPost("getDataTable")]
        public async Task<IActionResult> GetDiseasesTable(DataTableRequest request) {
            var result = await _profInfoService.GetDiseasesTable(request);

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

        [HttpGet("{diseaseId}")]
        public async Task<ActionResult<DiseaseDTO>> GetDisease(int diseaseId)
        {
            var disease = await _profInfoService.DiseaseAsync(diseaseId);

            return Ok(disease);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPut]
        public async Task<ActionResult<DiseaseDTO>> UpdateDisease(DiseaseDTO disease)
        {
            if (disease == null)
            {
                return BadRequest();
            }

            var dbdisease = await _profInfoService.UpdateDisease(disease, await GetUserName());

            if (dbdisease == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPost]
        public async Task<ActionResult<DiseaseDTO>> CreateDisease(DiseaseDTO disease)
        {
            var errors = await _profInfoService.ValidateDisease(disease);

            if (errors.Count() > 0)
            {
                return BadRequest(new ErrorResponse { Errors = errors, Title = "one or more validation errors occurred" });
            }

            var dbdisease = await _profInfoService.AddDiseaseAsync(disease, await GetUserName());

            return Ok(dbdisease);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpDelete("{DiseaseId}")]
        public async Task<ActionResult<DiseaseDTO>> DeleteDisease(int diseaseId)
        {
            if (diseaseId <= 0)
            {
                return BadRequest();
            }

            var disease = await _profInfoService.DeleteDisease(diseaseId);

            if (disease == null)
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