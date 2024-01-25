using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHeart.Core.Helpers;
using MyHeart.Services.Interfaces;
using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHeart.Data;
using Microsoft.EntityFrameworkCore;
using MyHeart.Api.Util;
using System.Security.Claims;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommercialNameController : ControllerBase
    {
        private readonly ICommercialNameService _profInfoService;
        private readonly IAuthenticationService _authenticationService;
        private readonly MyHeartContext _context;
        private readonly IUserService _userService;

        public CommercialNameController(ICommercialNameService profInfoService, IAuthenticationService authenticationService, MyHeartContext context, IUserService userService)
        {
            _authenticationService = authenticationService;
            _profInfoService = profInfoService;
            _context = context;
            _userService = userService;
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommercialNameDTO>>> GetAllCommercialNamesAsync()
        {
            var disease = await _profInfoService.CommercialNamesListAsync();

            return Ok(disease);
        }


        [HttpGet("{commercialNamesId}")]
        public async Task<ActionResult<CommercialNameDTO>> GetCommercialNames(int commercialNamesId)
        {
            var disease = await _profInfoService.CommercialNamesAsync(commercialNamesId);

            return Ok(disease);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPut]
        public async Task<ActionResult<CommercialNameDTO>> UpdateCommercialNames(CommercialNameDTO commercialNames)
        {
            if (commercialNames == null)
            {
                return BadRequest();
            }

            var dbdisease = await _profInfoService.UpdateCommercialNames(commercialNames, await GetUserName());

            if (dbdisease == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpPost]
        public async Task<ActionResult<CommercialNameDTO>> CreateCommercialNames(CommercialNameDTO commercialNames)
        {
            var errors = await _profInfoService.ValidateCommercialNames(commercialNames);

            if (errors.Count() > 0)
            {
                return BadRequest(new ErrorResponse { Errors = errors, Title = "one or more validation errors occurred" });
            }

            var dbCommercialNames = await _profInfoService.AddCommercialNamesAsync(commercialNames, await GetUserName());

            return Ok(dbCommercialNames);
        }

        [Authorize(Policy = Policies.MinAdmin)]
        [HttpDelete("{commercialNamesId}")]
        public async Task<ActionResult<CommercialNameDTO>> DeleteCommercialNames(int commercialNamesId)
        {
            if (commercialNamesId <= 0)
            {
                return BadRequest();
            }

            var disease = await _profInfoService.DeleteCommercialNames(commercialNamesId);

            if (disease == null)
            {
                return NotFound();
            }

            return Ok();
        }

        private async Task<string> GetUserName() {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.GetUserDetail(userId);
            return $"{user?.FirstName} {user?.LastName}";
        }
    }

}