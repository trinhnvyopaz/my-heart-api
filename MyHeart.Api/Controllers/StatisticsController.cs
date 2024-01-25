using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHeart.Api.Util;
using MyHeart.Services;
using MyHeart.Services.Interfaces;
using System.Threading.Tasks;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.MinAdmin)]
    public class StatisticsController : ControllerBase
    {
        private IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("pacients")]
        public async Task<ActionResult<byte[]>> ExportPacients()
        {
            var file = await _statisticsService.ExportPacients();

            return File(file.Content, file.MimeType);
        }

        [HttpGet("questions")]
        public async Task<ActionResult<byte[]>> ExportQuestions()
        {
            var file = await _statisticsService.ExportQuestions();

            return File(file.Content, file.MimeType);
        }


        [HttpGet("doctors")]
        public async Task<ActionResult<byte[]>> ExportDoctors()
        {
            var file = await _statisticsService.ExportDoctors();

            return File(file.Content, file.MimeType);
        }


        [HttpGet("diseases")]
        public async Task<ActionResult<byte[]>> ExportDiseases()
        {
            var file = await _statisticsService.ExportDiseases();

            return File(file.Content, file.MimeType);
        }

        [HttpGet("pharmacy")]
        public async Task<ActionResult<byte[]>> ExportPharmacy()
        {
            var file = await _statisticsService.ExportPharmacy();

            return File(file.Content, file.MimeType);
        }
        [HttpGet("nonpharmacy")]
        public async Task<ActionResult<byte[]>> ExportNonpharmacy()
        {
            var file = await _statisticsService.ExportNonpharmacy();

            return File(file.Content, file.MimeType);
        }
    }
}
