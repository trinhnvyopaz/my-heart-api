using System.Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MyHeart.Data;
using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SelectionController : ControllerBase
    {
        public readonly MyHeartContext _context;
        public readonly IMapper _mapper;

        public SelectionController(MyHeartContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("Diseases")]
        public async Task<ActionResult<DataTableResponse<Response>>> GetDiseases(
            [FromQuery] string? ids,
            [FromQuery] string? filter,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var query = _context.Disease.AsQueryable();
            if (filter != null)
            {
                query = query.Where(x => x.Name.Contains(filter));
            }
            if (ids != null)
            {
                query = query.Where(x => ids.Contains(x.Id.ToString()));
            }
            var total = await query.CountAsync();
            query = query
                .OrderByDescending(x => x.LastUpdateDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            return new DataTableResponse<Response>
            {
                Data = await query
                    .Select(x => new Response { Id = x.Id, Name = x.Name })
                    .ToListAsync(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        [HttpGet("Pharmaceuticals")]
        public async Task<
            ActionResult<DataTableResponse<ResponsePharmaceutical>>
        > GetPharmaceuticals(
            [FromQuery] string? filter,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var query = _context.Pharmacy.AsQueryable();
            if (filter != null)
            {
                query = query.Where(x => x.Name.Contains(filter));
            }
            var total = await query.CountAsync();
            query = query
                .OrderByDescending(x => x.LastUpdateDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            return new DataTableResponse<ResponsePharmaceutical>
            {
                Data = await query
                    .Select(
                        x =>
                            new ResponsePharmaceutical
                            {
                                Id = x.Id,
                                Name = $"{x.Name}-{x.Supplement}"
                            }
                    )
                    .ToListAsync(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        [HttpGet("PreventiveMeasures")]
        public async Task<ActionResult<DataTableResponse<Response>>> GetPreventiveMeasures(
            [FromQuery] string? filter,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var query = _context.PreventiveMeasures.AsQueryable();
            if (filter != null)
            {
                query = query.Where(x => x.Name.Contains(filter));
            }
            var total = await query.CountAsync();
            query = query
                .OrderByDescending(x => x.LastUpdateDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            return new DataTableResponse<Response>
            {
                Data = await query
                    .Select(x => new Response { Id = x.Id, Name = x.Name })
                    .ToListAsync(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        [HttpGet("NonpharmaticTherapies")]
        public async Task<ActionResult<DataTableResponse<Response>>> GetNonpharmaticTherapies(
            [FromQuery] string? filter,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var query = _context.NonpharmaticTherapy.AsQueryable();
            if (filter != null)
            {
                query = query.Where(x => x.Name.Contains(filter));
            }
            var total = await query.CountAsync();
            query = query
                .OrderByDescending(x => x.LastUpdateDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            return new DataTableResponse<Response>
            {
                Data = await query
                    .Select(x => new Response { Id = x.Id, Name = x.Name })
                    .ToListAsync(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }

    public class Response
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ResponsePharmaceutical : Response
    {
        public string Strength { get; set; }
        public string Package { get; set; }
    }
}
