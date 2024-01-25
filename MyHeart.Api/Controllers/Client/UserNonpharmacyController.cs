using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyHeart.Api.Util;
using MyHeart.DTO;
using MyHeart.DTO.User;
using MyHeart.Services;
using MyHeart.Services.Interfaces;
using MyHeart.Services.Interfaces.ProfessionInfo;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserNonpharmacyController : Controller
    {

        private IUserNonpharmacyService _userNonpharmacyService;
        private readonly IUserService _userService;

        public UserNonpharmacyController(IUserNonpharmacyService userNonpharmacyService, IUserService userService)
        {
            _userNonpharmacyService = userNonpharmacyService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNonpharmacies()
        {
            var result = await _userNonpharmacyService.GetUserNonpharmacies();

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [Authorize(Policy = Policies.MinDoctor)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserNonpharmacies(int id) {
            var result = await _userNonpharmacyService.GetUserNonpharmacies(id);

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SaveUserNonpharmacy([FromBody]UserNonpharmacyDto model)
        {
            if (model == null)
                return BadRequest();

            var currentUser = await _userService.CurrentUserAsync();
            if (currentUser.Id != model.UserId && currentUser.UserType == UType.Patient) {
                return Unauthorized();
            }

            var result = await _userNonpharmacyService.SaveUserNonpharmacy(model, await GetUserName());

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserNonpharmacy(int id)
        {
            var currentUser = await _userService.CurrentUserAsync();

            if (!(await _userNonpharmacyService.DoesNonPharmacyBelongToUser(currentUser.Id, id)) && currentUser.UserType == UType.Patient) {
                return Unauthorized();
            }

            var result = await _userNonpharmacyService.DeleteUserNonpharmacy(id);

            if (result)
                return Ok();

            return BadRequest();
        }

        [HttpGet("getNonpharmaticTherapiesByName/{searchString}")]
        public async Task<IActionResult> GetNonpharmaticTherapiesByName(string searchString)
        {
            var result = await _userNonpharmacyService.GetNonpharmaticTherapiesByName(searchString);

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