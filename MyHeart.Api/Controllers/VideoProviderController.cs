using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHeart.Services.Interfaces;
using System.Threading.Tasks;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VideoProviderController : Controller
    {
        private IVideoProviderService _videoProviderService;

        public VideoProviderController(IVideoProviderService videoProviderService)
        {
            _videoProviderService = videoProviderService;
        }

        [HttpGet("{questionId}")]
        public async Task<IActionResult> ConnectToRoom(int questionId)
        {
            var room = await _videoProviderService.ConnectToRoom(questionId);

            return Ok(room);
        }
    }
}
