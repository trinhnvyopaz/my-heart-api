using MyHeart.Core.Helpers;
using MyHeart.Data;
using MyHeart.Services.Interfaces;
using MyHeart.DTO.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;
using MyHeart.Data.Models;
using QRCoder;
using System.Drawing;
using System.IO;

namespace MyHeart.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly MyHeartContext _myHeartContext;
        private readonly SecuritySettings _securitySettings;
        private readonly IMFAService _mfaService;
        private readonly IUserService _userService;

        public AuthenticationService(MyHeartContext myHeartContext, IOptions<SecuritySettings> securitySettings, IMFAService mfaService, IUserService userService)
        {
            _myHeartContext = myHeartContext;
            _securitySettings = securitySettings.Value;
            _mfaService = mfaService;
            _userService = userService;
        }

        public async Task<LoginResponseDTO> Authenticate(LoginDTO login)
        {
            var user = await _myHeartContext.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Email == login.Email && u.EmailComfirmed == true);

            if (user == null)
                throw new Exception("NULLUSER");

            if (user.PasswordHash == "" || !PasswordHasher.CheckPassword(login.Password, user.PasswordHash, _securitySettings.Pepper))
                throw new Exception("WRONGPASSWORD");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_securitySettings.TokenSecret);

            var claimsIdentity = new ClaimsIdentity();
            var roles = user.Roles.Select(r => new Claim(ClaimTypes.Role, r.RoleType.ToString()));

            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            claimsIdentity.AddClaims(roles);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(_securitySettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResponseDTO() {
                Token = tokenHandler.WriteToken(token),
                MfaSecret = null
            };
        }

        public async Task<LoginResponseDTO> Authenticate(MFALoginDTO login) {
            var user = await _myHeartContext.Users
                .Where(u => u.Email == login.Email)
                .Include(u => u.Roles)
                .FirstOrDefaultAsync();

            if (user == null)
                throw new Exception("NULLUSER");

            if(!user.EmailComfirmed) {
                throw new Exception("EMAILCONFIRM");
            }

            if (!user.MFAConfirmed) {
                throw new Exception("MFACONFIRM");
            }

            if (!PasswordHasher.CheckPassword(login.Password, user.PasswordHash, _securitySettings.Pepper))
                throw new Exception("WRONGPASSWORD");

            /*if(! await _mfaService.ValidateCode(user.Id, login.MfaCode)) {
                throw new Exception("WRONGMFA");
            }*/

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_securitySettings.TokenSecret);
            var claimsIdentity = new ClaimsIdentity();
            var roles = user.Roles.Where(x => x.UserId == user.Id).Select(r => new Claim(ClaimTypes.Role, r.RoleType.ToString()));

            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            claimsIdentity.AddClaims(roles);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(_securitySettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            string sharedSecret = null;
            if (login.RememberMe) {
                var sharedSecretGuid = CryptoSafeGuid();
                sharedSecret = sharedSecretGuid.ToString();

                await _myHeartContext.TrustedLogins
                    .AddAsync(new UserTrustedLogin() {
                        LastUpdateUser = "SYSTEM",
                        SharedSecret = sharedSecretGuid,
                        UserId = user.Id
                    });

                await _myHeartContext.SaveChangesAsync();
            }

            return new LoginResponseDTO() {
                Token = tokenHandler.WriteToken(token),
                MfaSecret = sharedSecret
            };
        }

        public async Task<string> Refresh(UserDTO user) {
            if(user == null) {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_securitySettings.TokenSecret);

            var claimsIdentity = new ClaimsIdentity();
            var roles = await _myHeartContext.Roles.Where(x=>x.UserId == user.Id).Select(r => new Claim(ClaimTypes.Role, r.RoleType.ToString())).ToListAsync();

            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            claimsIdentity.AddClaims(roles);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(_securitySettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<string> CreateMobileLoginForUser() {
            var user = await _userService.CurrentUserAsync();

            if(user == null) {
                return null;
            }

            var secret = CryptoSafeGuid();

            await _myHeartContext.TrustedLogins
                    .AddAsync(new UserTrustedLogin() {
                        LastUpdateUser = "SYSTEM",
                        SharedSecret = secret,
                        UserId = user.Id
                    });
            await _myHeartContext.SaveChangesAsync();

            var message = $"{{\"email\":\"{user.Email}\",\"token\":\"{secret}\"}}";

            return MakeQrImageStringFromText(message);
        }

        public async Task<LoginResponseDTO> MobileLogin(MobileLoginDTO login) {
            var user = await _myHeartContext.Users.Where(x => x.Email == login.Email)
                .Include(x => x.TrustedLogins)
                .FirstOrDefaultAsync();

            if (user == null) {
                throw new Exception("NULLUSER");
            }

            var trustedLogin = user.TrustedLogins
                .Where(x => x.SharedSecret == login.Token)
                .FirstOrDefault();

            if(trustedLogin == null) {
                throw new Exception("BADTOKEN");
            }

            var newSecret = CryptoSafeGuid();
            await _myHeartContext.TrustedLogins
                .AddAsync(new UserTrustedLogin {
                    LastUpdateUser = "SYSTEM",
                    SharedSecret = newSecret,
                    UserId = user.Id
                });
            _myHeartContext.Remove(trustedLogin);
            await _myHeartContext.SaveChangesAsync();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_securitySettings.TokenSecret);
            var claimsIdentity = new ClaimsIdentity();
            var roles = user.Roles.Where(x => x.UserId == user.Id).Select(r => new Claim(ClaimTypes.Role, r.RoleType.ToString()));

            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            claimsIdentity.AddClaims(roles); ;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(_securitySettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResponseDTO {
                MfaSecret = newSecret.ToString(),
                Token = tokenHandler.WriteToken(token)
            };
        }

        public async Task<PhoneAuthDTO> RequestAuthorizeByPhone() {
            var secret = CryptoSafeGuid();

            var request = new UserPhoneAuthRequest() {
                CreatedAt = DateTime.UtcNow,
                LoginSecret = secret,
                PhoneAuthorized = false,
                AuthorisedUserId = -1
            };

            await _myHeartContext.PhoneAuth
                .AddAsync(request);

            await _myHeartContext.SaveChangesAsync();

            return new PhoneAuthDTO {
                Image = MakeQrImageStringFromText($"{{\"id\":\"{request.Id}\",\"secret\":\"{request.LoginSecret}\"}}"),
                Id = request.Id,
                Secret = request.LoginSecret
            };
        }

        public async Task<string> AuthorizeByPhone(UserDTO user, Guid secret, int id) {
            var request = await _myHeartContext
                .PhoneAuth
                .Where(x => x.Id == id && x.LoginSecret == secret)
                .FirstOrDefaultAsync();

            if(request == null) {
                return "INVALID";
            }

            if(request.CreatedAt.AddMinutes(10) < DateTime.UtcNow) {
                _myHeartContext.PhoneAuth.Remove(request);
                await _myHeartContext.SaveChangesAsync();
                return "STALE";
            }

            request.AuthorisedUserId = user.Id;
            request.PhoneAuthorized = true;
            await _myHeartContext.SaveChangesAsync();

            return "OK";
        }

        public async Task<LoginResponseDTO> CheckPhoneLogin(Guid secret, int id) {
            var request = await _myHeartContext
                .PhoneAuth
                .Where(x => x.Id == id && x.LoginSecret == secret)
                .FirstOrDefaultAsync();

            if (request == null) {
                throw new Exception("INVALID");
            }

            if (request.CreatedAt.AddMinutes(10) < DateTime.UtcNow) {
                _myHeartContext.PhoneAuth.Remove(request);
                await _myHeartContext.SaveChangesAsync();
                throw new Exception("STALE");
            }

            if(request.AuthorisedUserId == -1 || !request.PhoneAuthorized) {
                throw new Exception("WAIT");
            }

            _myHeartContext.PhoneAuth.Remove(request);
            await _myHeartContext.SaveChangesAsync();

            var user = await _userService.UserByIdAsync(request.AuthorisedUserId);
            if (user == null) {
                throw new Exception("BADUSER");
            }

            var newSecret = CryptoSafeGuid();
            await _myHeartContext.TrustedLogins
                .AddAsync(new UserTrustedLogin {
                    LastUpdateUser = "SYSTEM",
                    SharedSecret = newSecret,
                    UserId = request.AuthorisedUserId
                });
            await _myHeartContext.SaveChangesAsync();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_securitySettings.TokenSecret);
            var claimsIdentity = new ClaimsIdentity();
            var roles = _myHeartContext.Roles.Where(x => x.UserId == user.Id).Select(r => new Claim(ClaimTypes.Role, r.RoleType.ToString()));

            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            claimsIdentity.AddClaims(roles);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(_securitySettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResponseDTO {
                MfaSecret = newSecret.ToString(),
                Token = tokenHandler.WriteToken(token)
            };
        }

        /// Generates a cryptographically secure random Guid.
        ///
        /// Characteristics
        ///     - Variant: RFC 4122
        ///     - Version: 4
        /// RFC
        ///     https://tools.ietf.org/html/rfc4122#section-4.1.3
        /// Stackoverflow
        ///     https://stackoverflow.com/a/59437504/10830091
        public static Guid CryptoSafeGuid() {
            using var cryptoProvider = new RNGCryptoServiceProvider();

            // byte indices
            int versionByteIndex = BitConverter.IsLittleEndian ? 7 : 6;
            const int variantByteIndex = 8;

            // version mask & shift for `Version 4`
            const int versionMask = 0x0F;
            const int versionShift = 0x40;

            // variant mask & shift for `RFC 4122`
            const int variantMask = 0x3F;
            const int variantShift = 0x80;

            // get bytes of cryptographically-strong random values
            var bytes = new byte[16];
            cryptoProvider.GetBytes(bytes);

            // Set version bits -- 6th or 7th byte according to Endianness, big or little Endian respectively
            bytes[versionByteIndex] = (byte)(bytes[versionByteIndex] & versionMask | versionShift);

            // Set variant bits -- 9th byte
            bytes[variantByteIndex] = (byte)(bytes[variantByteIndex] & variantMask | variantShift);

            // Initialize Guid from the modified random bytes
            return new Guid(bytes);
        }

        public static string MakeQrImageStringFromText(string text, bool logoOverlay = true) {
            var qrGen = new QRCodeGenerator();
            var qrData = qrGen.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrData);

            Bitmap qrBitmap;
            if (logoOverlay) {
                qrBitmap = qrCode.GetGraphic(5, Color.Black, Color.White, (Bitmap)Image.FromFile("./wwwroot/Graphics/heart-pulse.png"));
            } else {
                qrBitmap = qrCode.GetGraphic(5);
            }

            return $"data:image/png;base64,{BitmapToBase64String(qrBitmap)}";
        }

        private static string BitmapToBase64String(Bitmap img) {
            using (MemoryStream stream = new MemoryStream()) {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return Convert.ToBase64String(stream.ToArray());
            }
        }
    }
}