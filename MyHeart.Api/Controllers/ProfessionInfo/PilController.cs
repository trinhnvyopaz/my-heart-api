using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyHeart.Api.Util;
using MyHeart.DTO;
using MyHeart.Services.Interfaces.ProfessionInfo;

namespace MyHeart.Api.Controllers.ProfessionInfo
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PilController : Controller
    {

        private IPilservice _pilService;

        public PilController(IPilservice pilService)
        {
            _pilService = pilService;
        }

        [HttpGet("{suklCode}")]
        public async Task<IActionResult> GetPillBySukl(int suklCode)
        {
            var result = await _pilService.GetPillBySukl(suklCode);

            return Ok(result);
        }

        [HttpGet("file/{suklCode}")]
        public async Task<IActionResult> GetFileBySukl(int suklCode) {
            var stream = await _pilService.GetFileStreamBySukl(suklCode);

            if(stream.file == null || stream.name == null) {
                return NotFound();
            }

            Response.Headers.Add("Content-Description", "File Transfer");
            Response.Headers.Add("Content-Disposition", "attachment; filename=\"|" + stream.name + "|\"");
            return File(stream.file, "application/octet-stream", stream.name);
        }
    }
}