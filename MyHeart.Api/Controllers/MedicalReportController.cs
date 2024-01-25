using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyHeart.Api.Util;
using MyHeart.DTO;
using MyHeart.DTO.MedicalReport;
using MyHeart.Services.Interfaces;
using System.Threading.Tasks;

namespace MyHeart.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicalReportController : ControllerBase
    {
        private readonly IPatientMedicalRecordService _patientMedicalRecordService;

        public MedicalReportController(
            IPatientMedicalRecordService patientMedicalRecordService
        )
        {
            _patientMedicalRecordService = patientMedicalRecordService;
        }

        [Authorize(Policy = Policies.MinDataManager)]
        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] int Page = 1,
            [FromQuery] int PageSize = 10,
            [FromQuery] string? Filter = ""
        )
        {
            var result = await _patientMedicalRecordService.Search(new DataTableRequest
            {
                Page = Page,
                PageSize = PageSize,
                Filter = Filter,
            });
            return Ok(result);
        }

        [Authorize(Policy = Policies.MinDataManager)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _patientMedicalRecordService.Get(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("export/{id}")]
        public async Task<ActionResult<byte[]>> ExportPharmacy(int id)
        {
            var file = await _patientMedicalRecordService.ExportExcel(id);

            return File(file.Content, file.MimeType);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string orcText)
        {
            PatientMedicalRecordDTO medicalReportDTO = new PatientMedicalRecordDTO();
            var result = await _patientMedicalRecordService.Create(medicalReportDTO, orcText);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] PatientMedicalRecordDTO medicalReportDTO
        )
        {
            var result = await _patientMedicalRecordService.Update(id, medicalReportDTO);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _patientMedicalRecordService.Delete(id);
            return Ok(result);
        }
    }
}
