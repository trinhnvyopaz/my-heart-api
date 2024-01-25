using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHeart.Services.Interfaces;
using MyHeart.DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHeart.Api.Util;
using MyHeart.DTO;
using MyHeart.DTO.Questions;
using System.Security.Claims;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly IUserService _userService;

        public QuestionController(IQuestionService questionService, IUserService userService)
        {
            _questionService = questionService;
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Policy = Policies.MinAdmin)]
        public async Task<ActionResult<List<QuestionListDTO>>> GetListAsync()
        {
            var list = await _questionService.GetListAsync();

            return Ok(list);
        }

        [HttpPost("getDataTable")]
        [Authorize(Policy = Policies.MinDoctor)]
        public async Task<IActionResult> GetDiseasesTable(GroupedDataTableRequest request) {
            var result = await _questionService.GetQuestionsTable(request);

            return Ok(result);
        }

        [HttpPost("open")]
        public async Task<ActionResult<List<QuestionListDTO>>> GetOpenQuestions(DataTableRequest request) {
            var user = await _userService.CurrentUserAsync();

            var questions = await _questionService.GetOpenUserQuestionsDataTable(user.Id, request);

            return Ok(questions);
        }

        [HttpPost("open/{id}")]
        [Authorize(Policy = Policies.MinDoctor)]
        public async Task<ActionResult<List<QuestionListDTO>>> GetOpenQuestions(int id, [FromBody] DataTableRequest request) {
            var questions = await _questionService.GetOpenUserQuestionsDataTable(id, request);

            return Ok(questions);
        }

        [HttpPost("closed")]
        public async Task<ActionResult<List<QuestionListDTO>>> GetClosedQuestions(DataTableRequest request) {
            var user = await _userService.CurrentUserAsync();

            var questions = await _questionService.GetClosedUserQuestionsDataTable(user.Id, request);

            return Ok(questions);
        }

        [HttpPost("closed/{id}")]
        [Authorize(Policy = Policies.MinDoctor)]
        public async Task<ActionResult<List<QuestionListDTO>>> GetClosedQuestions(int id, [FromBody] DataTableRequest request) {
            var questions = await _questionService.GetClosedUserQuestionsDataTable(id, request);

            return Ok(questions);
        }

        [HttpPost("ask")]
        public async Task<ActionResult<QuestionListDTO>> AskQuestion(QuestionListDTO question) {
            var user = await _userService.CurrentUserAsync();

            if (question.UserId != user.Id && question.UserId != -1) {
                return BadRequest("Nemůžete se ptát za jiného uživatele");
            }
            if (string.IsNullOrEmpty(question.Subject)) {
                return BadRequest("Předmět nemůže být prázdný");
            }
            if (string.IsNullOrEmpty(question.Body)) {
                return BadRequest("Zpráva nemůže být prázdná");
            }

            if(question.Id != -1) {
                var dbQuestion = await _questionService.GetQuestionById(question.Id);

                if(question.UserId == dbQuestion.UserId && question.Subject == dbQuestion.Subject) {
                    return BadRequest("Tento dotaz už jste zadali");
                }
            }

            var questions = await _questionService.AskQuestion(question.UserId != -1 ? question.UserId : user.Id, question, await GetUserName());

            return Ok(questions);
        }

        [HttpGet("archive/{questionId}")]
        public async Task<ActionResult> ArchiveQuestion(int questionId) {
            var user = await _userService.CurrentUserAsync();

            if(user.UserType == UType.Patient && !(await _questionService.DoesQuestionBelongToUser(questionId, user.Id))) {
                return Unauthorized("Uživatelé mohou archivovat pouze své dotazy");
            }

            return Ok(await _questionService.ArchiveQuestion(questionId, user));
        }

        [HttpGet("{QuestionId}")]
        public async Task<ActionResult<QuestionListDTO>> GetQuestionById(int QuestionId) {
            var user = await _userService.CurrentUserAsync();
            
            if(user.UserType == UType.Patient && !(await _questionService.DoesQuestionBelongToUser(QuestionId, user.Id))) {
                return Unauthorized("Můžete přitupovat jen ke svým dotazům");
            }

            var question = await _questionService.GetQuestionById(QuestionId);

            return Ok(question);
        }

        [HttpPost("lastReplies")]
        public async Task<ActionResult<DataTableResponse<QuestionCommentDTO>>> GetLastCommentsOfQuestion(QuestionCommentRequest request) {
            var user = await _userService.CurrentUserAsync();

            if (user.UserType == UType.Patient && !(await _questionService.DoesQuestionBelongToUser(request.QuestionId, user.Id))) {
                return Unauthorized("Můžete přitupovat jen ke svým dotazům");
            }

            var responses = await _questionService.GetLastCommentsOfQuestion(request.QuestionId, request.Page, request.PageSize);

            return Ok(responses);
        }

        [HttpGet("video/{id}")]
        public async Task<ActionResult<VideoRoomDTO>> GetVideoRoomDetails(int id) {
            var user = await _userService.CurrentUserAsync();
            if(!(await _questionService.DoesQuestionBelongToUser(id, user.Id)) && user.UserType == UType.Patient) {
                return Unauthorized();
            }

            var room = await _questionService.GetVideoRoomDetails(id);

            if(room == null) {
                return BadRequest("Špatné ID dotazu");
            }

            return room;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var result = await _questionService.DeleteQuestion(id);
            if(result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        private async Task<string> GetUserName() {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.GetUserDetail(userId);
            return $"{user?.FirstName} {user?.LastName}";
        }
    }
}