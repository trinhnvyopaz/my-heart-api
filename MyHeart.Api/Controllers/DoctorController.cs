using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHeart.Core.Helpers;
using MyHeart.Services.Interfaces;
using MyHeart.DTO;
using MyHeart.DTO.User;
using MyHeart.DTO.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHeart.Data;
using Microsoft.EntityFrameworkCore;
using MyHeart.Data.Models;
using MyHeart.Api.Util;
using System.Security.Claims;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.MinDoctor)]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IAuthenticationService _authenticationService;
        private readonly MyHeartContext _context;
        private readonly IUserService _userService;

        public DoctorController(IDoctorService doctorService, IAuthenticationService authenticationService, MyHeartContext context, IUserService userService)
        {
            _authenticationService = authenticationService;
            _doctorService = doctorService;
            _context = context;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDoctorDTO registerViewModel)
        {
            var errors = await _doctorService.Validate(registerViewModel);

            if (errors.Count() > 0)
            {
                return BadRequest(new ErrorResponse { Errors = errors, Title = "one or more validation errors occurred" });
            }
            var user = await _doctorService.RegisterUserAsync(registerViewModel);

            return Ok(user);
        }

        [HttpGet("ByUserId/{id}")]
        public async Task<ActionResult<DoctorDetailDTO>> GetDoctorDetailByUserId(int id)
        {
            var doctor = await _doctorService.GetDoctorDetailByUserID(id);
            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }

        [HttpGet("Current")]
        public async Task<ActionResult<DoctorDetailDTO>> GetCurrentDoctorDetail() {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);

            var doctor = await _doctorService.GetDoctorDetailByUserID(userId);

            return Ok(doctor);
        }

        [HttpGet("{doctorId}")]
        public async Task<ActionResult<DoctorDetailDTO>> GetDoctorDetail(int doctorId)
        {
            var detail = await _doctorService.GetDoctorDetail(doctorId);

            return Ok(detail);

        }

        [HttpPatch("Update")]
        public async Task<ActionResult<DoctorDetailDTO>> UpdateDoctor(DoctorDetailDTO doctor)
        {
            if (doctor == null)
            {
                return BadRequest();
            }

            var dbuser = await _doctorService.UpdateDoctor(doctor, await GetUserName());

            if (dbuser == null)
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