using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyHeart.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : Controller
    {
        private IImportService _importService;

        public ImportController(IImportService importService)
        {
            _importService = importService;
        }

        [HttpPost]
        public async Task<IActionResult> ImportFile(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest();
            }

            string extension = Path.GetExtension(file.FileName);

            if (extension != ".xlsx" && extension != ".xls")
            {
                return BadRequest();
            }

            await _importService.ImportData(file);

            return Ok();
        }
    }
}