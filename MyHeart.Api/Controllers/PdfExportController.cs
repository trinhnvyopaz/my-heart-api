using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHeart.Core.Helpers;
using MyHeart.Services.Interfaces;
using MyHeart.DTO;
using MyHeart.DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHeart.Data;
using Microsoft.EntityFrameworkCore;
using MyHeart.Data.Models;
using MyHeart.Api.Util;
using MyHeart.DTO.Questions;
using System.Text.RegularExpressions;
using MyHeart.Services;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfExportController : ControllerBase
    {
        private readonly IPdfReportService _pdfReportService;

        public PdfExportController(IPdfReportService pdfReportService)
        {
            _pdfReportService = pdfReportService;
        }

        [HttpGet("HealtStatusReport/{userId}")]
        public async Task<IActionResult> GetHealthStatusReport(int userId)
        {
            var file = await _pdfReportService.GetHealthsStatusReportAsync(userId);

            if (file == null)
                return BadRequest();

            return File(file.Content, file.MimeType);
        }
        [HttpGet("PharmaciesReport/{userId}")]
        public async Task<IActionResult> GetPharmaciesReport(int userId)
        {
            var file = await _pdfReportService.GetPharmaciesReportAsync(userId);

            if (file == null)
                return BadRequest();

            return File(file.Content, file.MimeType);
        }
        [HttpGet("ChatReport/{questionId}/{userId}")]
        public async Task<IActionResult> GetChatReport(int questionId, int userId)
        {
            var file = await _pdfReportService.GetChatReportDto(userId, questionId);

            if (file == null)
                return BadRequest();

            return File(file.Content, file.MimeType);
        }
    }
}