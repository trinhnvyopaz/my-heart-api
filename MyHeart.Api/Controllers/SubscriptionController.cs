using MyHeart.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyHeart.Data;
using MyHeart.Services;
using System.Threading.Tasks;
using System.Collections;
using MyHeart.DTO;

namespace MyHeart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        private ISubscriptionService _subscriptionService;
        private IUserService _userService;

        public SubscriptionController(ISubscriptionService subscriptionService, IUserService userService)
        {
            _subscriptionService = subscriptionService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSubscriptions()
        {
            var list = await _subscriptionService.GetSubscriptions();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubscriptions(int id)
        {
            var subscription = await _subscriptionService.GetSubscription(id);

            if(subscription == null)
            {
                return NotFound();
            }

            return Ok(subscription);
        }
    }
}