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
    public class AtcController : Controller
    {
        private IAtcService _atcService;

        public AtcController(IAtcService atcService)
        {
            _atcService = atcService;
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpGet]
        public async Task<IActionResult> GetAllAtcs()
        {
            var result = await _atcService.GetAllAtcs();

            return Ok(result);
        }

        [HttpPost("getPagedResource")]
        public async Task<IActionResult> GetPagedResource(SortedPagedSourceRequest request) {
            var result = await _atcService.GetPagedResource(request);

            return Ok(result);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPost("getByIds")]
        public async Task<IActionResult> GetByIds(IEnumerable<int> ids) {
            var result = await _atcService.GetByIds(ids);

            return Ok(result);
        }
    }
}