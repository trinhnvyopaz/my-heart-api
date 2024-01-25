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
using MyHeart.DTO.User;
using MyHeart.Api.Util;
using MyHeart.DTO.ProfessionInformation;
using MyHeart.Data.Models;
using System.Security.Claims;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.MinAdmin)]
    public class SymptomQuestionsController : ControllerBase
    {
        private readonly ISymptomQuestionsService _profInfoService;
        private readonly IAuthenticationService _authenticationService;
        private readonly MyHeartContext _context;
        private readonly IUserService _userService;

        public SymptomQuestionsController(ISymptomQuestionsService profInfoService, IAuthenticationService authenticationService, MyHeartContext context, IUserService userService)
        {
            _authenticationService = authenticationService;
            _profInfoService = profInfoService;
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SymptomQuestionsDTO>>> GetAllSymptomQuestionsAsync()
        {
            var disease = await _profInfoService.SymptomQuestionsListAsync();

            return Ok(disease);
        }

        [HttpPost("getDataTable")]
        public async Task<IActionResult> GetSymptomQuestionsTable(DataTableRequest request)
        {
            var result = await _profInfoService.GetSymptomQuestionsTable(request);

            return Ok(result);
        }

        [HttpGet("{diseaseId}")]
        public async Task<ActionResult<SymptomQuestionsDTO>> GetSymptomQuestions(int diseaseId)
        {
            var disease = await _profInfoService.SymptomQuestionsAsync(diseaseId);

            return Ok(disease);
        }
        [HttpGet("disease/{diseaseID}")]
        public async Task<ActionResult<IEnumerable<SymptomQuestionsDTO>>> GetSymptomQuestionsByDisease(int diseaseId)
        {
            var symptomQuestions = await _profInfoService.SymptomQuestionsListByDiseaseIdAsync(diseaseId);

            return Ok(symptomQuestions);
        }

        [HttpPut]
        public async Task<ActionResult<SymptomQuestionsDTO>> UpdateSymptomQuestions(SymptomQuestionsDTO disease)
        {
            if (disease == null)
            {
                return BadRequest();
            }

            var dbdisease = await _profInfoService.UpdateSymptomQuestions(disease, await GetUserName());

            if (dbdisease == null)
            {
                return NotFound();
            }

            return Ok(dbdisease);
        }


        [HttpPost]
        public async Task<ActionResult<SymptomQuestionsDTO>> CreateSymptomQuestions(SymptomQuestionsDTO disease)
        {
            var errors = await _profInfoService.ValidateSymptomQuestions(disease);

            if (errors.Count() > 0)
            {
                return BadRequest(new ErrorResponse { Errors = errors, Title = "one or more validation errors occurred" });
            }

            try
            {
                var dbdisease = await _profInfoService.AddSymptomQuestionsAsync(disease, await GetUserName());
                return Ok(dbdisease);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }

          
        }

        [HttpDelete("{DiseaseId}")]
        public async Task<ActionResult<SymptomQuestionsDTO>> DeleteSymptomQuestions(int diseaseId)
        {
            if (diseaseId <= 0)
            {
                return BadRequest();
            }

            var disease = await _profInfoService.DeleteSymptomQuestions(diseaseId);

            if (disease == null)
            {
                return NotFound();
            }

            return Ok();
        }
        [HttpPost("getQuestions")]
        public async Task<IActionResult> GetSymptomQuestionListBySymptomIds(IEnumerable<int> ids)
        {
            var questions = await _profInfoService.GetSymptomQuestionsBySymptomIds(ids);

            return Ok(questions);
        }
        [HttpPut("updateSymtomQuestionDiseases")]
        public async Task<IActionResult> UpdateSymptomQuestionsDiseases(IEnumerable<SymptomQuestions_DiseaseDTO> diseases)
        {
            var questionDiseases = await _profInfoService.UpdateSymptomQuestionsDiseases(diseases, await GetUserName());

            return Ok(questionDiseases);
        }

        [HttpPost("getPagedResource")]
        public async Task<IActionResult> GetPagedResource(SortedPagedSourceRequest request)
        {
            var result = await _profInfoService.GetPagedResource(request);

            return Ok(result);
        }

        [HttpPost("getByIds")]
        public async Task<IActionResult> GetByIds(IEnumerable<int> ids)
        {
            var questions = await _profInfoService.GetSymptomQuestionsByIds(ids);
            return Ok(questions);
        }

        private async Task<string> GetUserName()
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.GetUserDetail(userId);
            return $"{user.FirstName} {user.LastName}";
        }
    }

}