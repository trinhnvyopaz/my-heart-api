using MyHeartAdvice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyHeartAdvice.Controllers {
  [AllowAnonymous]
  [Route("login")]
  public class Testing : Controller {
    private readonly ILoginService _loginService;

    public Testing(ILoginService loginService) {
      _loginService = loginService;
    }

    [HttpPost]
    public IActionResult Login([FromBody]dynamic loginModel) {
      if (string.IsNullOrEmpty(loginModel.userName.Value) || string.IsNullOrEmpty(loginModel.password.Value))
        return BadRequest();

      var logged = _loginService.Login(loginModel);
      string token = null;

      if (logged == true)
        token = _loginService.CreateJwtToken();

      if (string.IsNullOrEmpty(token))
        return Unauthorized();

      return Ok(token);
    }
  }
}