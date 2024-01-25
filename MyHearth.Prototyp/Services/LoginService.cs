using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MyHeartAdvice.Services {
  public class LoginService : ILoginService {
    public const string Issuer = "www.memos.cz";
    public const string Audience = "MyHeartAdvice";
    const int TokenExpirationInMinutes = 14 * 24 * 60; // 14 dní

    public LoginService() {
    }

    public bool Login(dynamic loginModel) {
      if (loginModel.userName.Value.ToLower() == "admin" && loginModel.password.Value == "123456") {
        return true;
      }
      else
        return false;
    }

    public string CreateJwtToken() {
      //Create the list of claims
      var claims = new[]
      {
                new Claim(ClaimTypes.Name, "admin")
            };

      //Create the JWT token
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("e4%84*378q6d&f£ae£$F675**00fssDG^6*"));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: Issuer,
          audience: Audience,
          claims: claims,
          expires: DateTime.UtcNow.AddMinutes(TokenExpirationInMinutes),
          signingCredentials: creds);

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}