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
    public class SymptomController : ControllerBase
    {
        private readonly ISymptomService _profInfoService;
        private readonly IAuthenticationService _authenticationService;
        private readonly MyHeartContext _context;
        private readonly IUserService _userService;

        public SymptomController(ISymptomService profInfoService, IAuthenticationService authenticationService, MyHeartContext context, IUserService userService)
        {
            _authenticationService = authenticationService;
            _profInfoService = profInfoService;
            _context = context;
            _userService = userService;
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SymptomDTO>>> GetAllSymptomsAsync()
        {
            var symptoms = await _profInfoService.SymptomsListAsync();

            return Ok(symptoms);
        }

        [HttpPost("getDataTable")]
        public async Task<IActionResult> GetSymptomQuestionsTable(DataTableRequest request) {
            var result = await _profInfoService.GetSymptomsTable(request);

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

        [HttpGet("{SymptomId}")]
        public async Task<ActionResult<SymptomDTO>> GetSymptom(int symptomId)
        {
            var symptom = await _profInfoService.SymptomAsync(symptomId);

            return Ok(symptom);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPatch]
        public async Task<ActionResult<SymptomDTO>> UpdateSymptom(SymptomDTO symptom)
        {
            if (symptom == null)
            {
                return BadRequest();
            }

            var dbSymptom = await _profInfoService.UpdateSymptom(symptom, await GetUserName());

            if (dbSymptom == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPost]
        public async Task<ActionResult<SymptomDTO>> CreateSymptom(SymptomDTO symptom)
        {
            var errors = await _profInfoService.ValidateSymptom(symptom);

            if (errors.Count() > 0)
            {
                return BadRequest(new ErrorResponse { Errors = errors, Title = "one or more validation errors occurred" });
            }
            var dbSymptom = await _profInfoService.AddSymptomAsync(symptom, await GetUserName());

            return Ok(dbSymptom);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpDelete("{symptomId}")]
        public async Task<ActionResult<SymptomDTO>> DeleteSymptom(int symptomId)
        {
            if (symptomId <= 0)
            {
                return BadRequest();
            }

            var symptom = await _profInfoService.DeleteSymptom(symptomId);

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