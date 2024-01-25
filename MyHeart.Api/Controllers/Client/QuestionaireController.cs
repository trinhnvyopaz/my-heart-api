using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHeart.DTO;
using MyHeart.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHeart.Api.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionaireController : Controller
    {
        private IQuestionaireService _questionaireService;

        public QuestionaireController(IQuestionaireService questionaireService)
        {
            _questionaireService = questionaireService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestionaire(Client_QuestionaireDTO questionaire)
        {
            var dbQuestionaire = await _questionaireService.CreateQuestionaire(questionaire);

            return Ok(dbQuestionaire);
        }
        [HttpPost("getDataTable")]
        public async Task<IActionResult> GetQuestionaire(DataTableRequest request)
        {
            var dbQuestionaires = await _questionaireService.GetQuestionaireTable(request);

            return Ok(dbQuestionaires);
        }

        [HttpGet("getQuestions/{id}")]
        public async Task<IActionResult> GetQuestions(int id)
        {
            var questions = await _questionaireService.GetQuestions(id);

            return Ok(questions);
        }
        [HttpGet("result/{id}")]
        public async Task<IActionResult> GetResult(int id)
        {
            var result = await _questionaireService.GetResult(id);

            return Ok(result);
        }
    }
}
