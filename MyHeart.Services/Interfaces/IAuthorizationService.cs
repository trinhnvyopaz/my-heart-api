using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<LoginResponseDTO> Authenticate(LoginDTO login);
        public Task<LoginResponseDTO> Authenticate(MFALoginDTO login);
        public Task<string> Refresh(UserDTO user);
        public Task<string> CreateMobileLoginForUser();
        public Task<LoginResponseDTO> MobileLogin(MobileLoginDTO login);
        public Task<string> AuthorizeByPhone(UserDTO user, Guid secret, int id);
        public Task<PhoneAuthDTO> RequestAuthorizeByPhone();
        public Task<LoginResponseDTO> CheckPhoneLogin(Guid secret, int id);
    }
}