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
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.AspNetCore.Server.IIS.Core;
using System.IO;
using static System.Net.WebRequestMethods;
using MyHeart.Api.Util;
using Org.BouncyCastle.Ocsp;
using System.Security.Claims;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnamnesisController : ControllerBase
    {
        private readonly IAnamnesisService _anamnesisService;
        private readonly IUserService _userService;

        public AnamnesisController(IAnamnesisService anamnesisService, IUserService userService)
        {
            _anamnesisService = anamnesisService;
            _userService = userService;
        }

        [HttpGet("getPharmacyAnamnesis")]
        public async Task<IActionResult> GetPharmacyAnamnesis()
        {
            var result = await _anamnesisService.GetPersonalPharmacy();

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpGet("getPharmacyAnamnesis/{id}")]
        public async Task<IActionResult> GetPharmacyAnamnesis(int id) {
            var result = await _anamnesisService.GetPersonalPharmacy(id);

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpPost("savePharmacyAnamnesis")]
        public async Task<IActionResult> SavePharmacyAnamnesy([FromBody]UserAnamnesisDTO model)
        {
            var currentUser = await _userService.CurrentUserAsync();
            if (currentUser.Id != model.UserId && currentUser.UserType == UType.Patient) {
                return Unauthorized();
            }

            var result = await _anamnesisService.SavePharmacyAnamnesis(model, await GetUserName());

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpGet("getPharmacyByNameReg/{nameReg}")]
        public async Task<IActionResult> GetPharmacyByNameReg(string nameReg)
        {
            var result = await _anamnesisService.GetPharmacyByNameReg(nameReg);

            if (result == null)
                return BadRequest();

            return Ok(result);
        }
        [HttpGet("getPharmacyByNameRegAndStrength/{nameReg}/{strength}")]
        public async Task<IActionResult> GetPharmacyByNameRegAndStrength(string nameReg, string strength)
        {
            var result = await _anamnesisService.GetPharmacyByNameRegStrength(nameReg, strength);

            if (result == null)
                return BadRequest();

            return Ok(result);
        }
        [HttpDelete("deletePharmacyAnamnesis/{id}")]
        public async Task<IActionResult> DeletePharmacyAnamnesis(int id)
        {
            var user = await _userService.CurrentUserAsync();
            if(!(await _anamnesisService.DoesAnamnesisBelongToUser(user.Id, id)) && user.UserType == UType.Patient){
                return Unauthorized();
            }

            await _anamnesisService.DeletePharmacyAnamnesis(id);
            return Ok();
        }

        [HttpGet("getPersonalAnamneses")]
        public async Task<IActionResult> GetPersonalAnamneses()
        {
            var result = await _anamnesisService.GetPersonalAnamnesis();

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpGet("getPersonalAnamnesesByType/{type}")]
        public async Task<IActionResult> GetPersonalAnamneses(PersonalAnamnesisType type)
        {
            var result = await _anamnesisService.GetPersonalAnamnesisByType(type);

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpGet("getPersonalAnamneses/{id}")]
        public async Task<IActionResult> GetPersonalAnamneses(int id) {
            var result = await _anamnesisService.GetPersonalAnamnesis(id);

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpPost("savePersonalAnamnesis")]
        public async Task<IActionResult> SavePersonalAnamnesis([FromBody]UserAnamnesisDTO model)
        {
            var user = await _userService.CurrentUserAsync();

            if(user.Id != model.UserId && user.UserType == UType.Patient) {
                return Unauthorized();
            }

            var result = await _anamnesisService.SavePersonalAnamnesis(model, await GetUserName());

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpDelete("deletePersonalAnamnesis/{id}")]
        public async Task<IActionResult> DeletePersonalAnamnesis(int id)
        {
            var user = await _userService.CurrentUserAsync();
            if (!(await _anamnesisService.DoesAnamnesisBelongToUser(user.Id, id)) && user.UserType == UType.Patient) {
                return Unauthorized();
            }

            if (await _anamnesisService.DeletePersonalAnamnesis(id))
                return Ok();

            return BadRequest();
        }

        [HttpPut("updatePersonalAnamnesis/{id}")]
        public async Task<IActionResult> UpdatePersonalAnamnesis([FromBody]UserAnamnesisDTO userAnamnesis)
        {
            var user = await _userService.CurrentUserAsync();

            if (user.Id != userAnamnesis.UserId && user.UserType == UType.Patient)
            {
                return Unauthorized();
            }

            var result = await _anamnesisService.UpdatePersonalAnamnesis(userAnamnesis, await GetUserName());

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpGet("exportPersonalAnamnesis")]
        public async Task<IActionResult> ExportPersonalAnamnesis()
        {
            var result = await _anamnesisService.GetPersonalAnamnesisCsv();

            Response.Headers.Add("Content-Description", "File Transfer");
            Response.Headers.Add("Content-Disposition", "attachment; filename=\"personal-anamnesis.csv\"");
            return File(result, "application/CSV", $"personal-anamnesis.csv");
        }

        [HttpGet("getAbususAnamneses")]
        public async Task<IActionResult> GetAbususAnamneses()
        {
            var result = await _anamnesisService.GetAbususAnamnesis();

            return Ok(result);
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpGet("getAbususAnamneses/{id}")]
        public async Task<IActionResult> GetAbususAnamneses(int id) {
            var result = await _anamnesisService.GetAbususAnamnesis(id);

            return Ok(result);
        }

        [HttpPost("saveAbususAnamnesis")]
        public async Task<IActionResult> SaveAbususAnamnesis([FromBody]UserAnamnesisDTO model)
        {
            var user = await _userService.CurrentUserAsync();
            if (user.Id != model.UserId && user.UserType == UType.Patient) {
                return Unauthorized();
            }

            var result = await _anamnesisService.SaveAbususAnamnesis(model, await GetUserName());

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpGet("getFamilyAnamneses")]
        public async Task<IActionResult> GetFamilyAnamneses()
        {
            var result = await _anamnesisService.GetFamilyAnamnesis();

            return Ok(result);
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpGet("getFamilyAnamneses/{id}")]
        public async Task<IActionResult> GetFamilyAnamneses(int id) {
            var result = await _anamnesisService.GetFamilyAnamnesis(id);

            return Ok(result);
        }

        [HttpPost("saveFamilyAnamnesis")]
        public async Task<IActionResult> SaveFamilyAnamnesis([FromBody]UserAnamnesisDTO model)
        {
            var user = await _userService.CurrentUserAsync();
            if (user.Id != model.UserId && user.UserType == UType.Patient) {
                return Unauthorized();
            }

            var result = await _anamnesisService.SaveFamilyAnamnesis(model, await GetUserName());

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpGet("getSocialAnamneses")]
        public async Task<IActionResult> GetSocialAnamneses()
        {
            var result = await _anamnesisService.GetSocialAnamnesis();

            return Ok(result);
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpGet("getSocialAnamneses/{id}")]
        public async Task<IActionResult> GetSocialAnamneses(int id) {
            var result = await _anamnesisService.GetSocialAnamnesis();

            return Ok(result);
        }

        [HttpPost("saveSocialAnamnesis")]
        public async Task<IActionResult> SaveSocialAnamnesis([FromBody]UserAnamnesisDTO model)
        {
            if (model == null)
                return BadRequest();

            var user = await _userService.CurrentUserAsync();
            if (user.Id != model.UserId && user.UserType == UType.Patient) {
                return Unauthorized();
            }

            var result = await _anamnesisService.SaveSocialAnamnesis(model, await GetUserName());

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpGet("getAllergyAnamneses")]
        public async Task<IActionResult> GetAllergyAnamneses()
        {
            var result = await _anamnesisService.GetAllergyAnamneses();

            return Ok(result);
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpGet("getAllergyAnamneses/{id}")]
        public async Task<IActionResult> GetAllergyAnamneses(int id) {
            var result = await _anamnesisService.GetAllergyAnamneses(id);

            return Ok(result);
        }

        [HttpPost("saveAllergyAnamnesis")]
        public async Task<IActionResult> SaveAllergyAnamneis([FromBody] UserAnamnesisDTO model)
        {
            if (model == null)
                return BadRequest();

            var user = await _userService.CurrentUserAsync();
            if (user.Id != model.UserId && user.UserType == UType.Patient) {
                return Unauthorized();
            }

            var result = await _anamnesisService.SaveAllergyAnamneses(model, await GetUserName());

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpDelete("deleteAllergyAnamnesis/{id}")]
        public async Task<IActionResult> DeleteAllergyANamnesis(int id)
        {
            var user = await _userService.CurrentUserAsync();
            if (!(await _anamnesisService.DoesAnamnesisBelongToUser(user.Id, id)) && user.UserType == UType.Patient) {
                return Unauthorized();
            }

            if (await _anamnesisService.DeleteAllergyAnamnesis(id))
                return Ok();

            return BadRequest();
        }

        [HttpGet("getDiseasesByName/{searchString}")]
        public async Task<IActionResult> GetDiseasesByName(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
                return BadRequest();

            var result = await _anamnesisService.GetDiseasesByName(searchString);

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpGet("getDiseaseAnamneses")]
        public async Task<IActionResult> GetDiseaseAnamneses()
        {
            var result = await _anamnesisService.GetDiseaseAnamneses();

            if (result == null)
                return BadRequest();

            return Ok(result);
        }


        [HttpGet("getAllDiseases")]
        public async Task<IActionResult> GetAllDiseases()
        {
            return Ok(await _anamnesisService.GetAllDieseases());
        }

        [AllowAnonymous]
        [HttpPost("getDiseases")]
        public async Task<IActionResult> GetDiseases([FromBody]DataTableRequest request)
        {
            if (request == null)
                return BadRequest();

            var result = await _anamnesisService.GetDiseases(request);

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpGet("getDiseaseAnamneses/{id}")]
        public async Task<IActionResult> GetDiseaseAnamneses(int id) {
            var result = await _anamnesisService.GetDiseaseAnamneses(id);

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpPost("saveDiseaseAnamnesis")]
        public async Task<IActionResult> SaveDiseaseAnamnesis([FromBody] UserDiseaseDto model)
        {
            if (model == null)
                return null;

            var user = await _userService.CurrentUserAsync();
            if (user.Id != model.UserId && user.UserType == UType.Patient) {
                return Unauthorized();
            }

            var result = await _anamnesisService.SaveDiseaseAnamnesis(model, await GetUserName());

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpDelete("deleteDiseaseAnamnesis/{id}")]
        public async Task<IActionResult> DeleteDiseaseAnamnesis(int id)
        {
            if (id < 1)
                return BadRequest();

            var user = await _userService.CurrentUserAsync();
            if (!(await _anamnesisService.DoesAnamnesisBelongToUser(user.Id, id)) && user.UserType == UType.Patient) {
                return Unauthorized();
            }

            if (await _anamnesisService.DeleteDiseaseAnamnesis(id))
                return Ok();

            return BadRequest();
        }


        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllAnamnesis()
        {
            IEnumerable<UserAnamnesisDTO> result = await _anamnesisService.GetAllAnamneses();

            return Ok(result);
        }
        [HttpGet("getGeneralAnamneses")]
        public async Task<IActionResult> GetGeneralAnamneses()
        {
            var result = await _anamnesisService.GetGeneralAnamnesis();

            return Ok(result);
        }

        [HttpPost("saveGeneralAnamnesis")]
        public async Task<IActionResult> SaveGeneralAnamneis([FromBody] UserAnamnesisDTO model)
        {
            if (model == null)
                return BadRequest();

            var user = await _userService.CurrentUserAsync();
            if (user.Id != model.UserId && user.UserType == UType.Patient)
            {
                return Unauthorized();
            }

            var result = await _anamnesisService.SaveGeneralAnamnesis(model, await GetUserName());

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        private async Task<string> GetUserName() {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.GetUserDetail(userId);
            return $"{user?.FirstName} {user?.LastName}";
        }
    }
}
