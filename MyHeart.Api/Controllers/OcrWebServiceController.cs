using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyHeart.Core.Helpers;
using MyHeart.Data;
using MyHeart.DTO.OCRWebService;
using MyHeart.Services.Interfaces;

namespace MyHeart.Api.Controllers
{
    [Route("[controller]")]
    public class OcrWebServiceController : ControllerBase
    {
        private readonly ILogger<OcrWebServiceController> _logger;
        private readonly MyHeartContext _context;
        private readonly IMapper _mapper;
        private readonly IOCRWebService _service;

        public OcrWebServiceController(
            ILogger<OcrWebServiceController> logger,
            MyHeartContext context,
            IMapper mapper,
            IOCRWebService service
        )
        {
            _context = context;
            _logger = logger;
            _service = service;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> GetContent(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest();
            }

            string extension = Path.GetExtension(file.FileName);

            // if (extension != ".pdf" && extension != ".jpg")
            // {
            //     return BadRequest();
            // }
            var result = await _service.ProcessDocument(file);
            return Ok(result);
        }
    }
}
