using MyHeart.MobileApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using MyHeart.MobileApp.Dto;
using MyHeart.DTO.User;
using MyHeart.DTO;

namespace MyHeart.MobileApp.Services
{
    public class LoginService : BaseService
    {

        public async Task<bool> LogOut()
        {
            return true;
        }
        public async Task<ApiResponse<LoginResponseDTO>> MfaLogin(LoginViewModel vm)
        {
            var dto = MapVmToMFADto(vm);
            var url = ServiceConfig.BaseURL + "User/mfaLogin";
            var response = await restService.SendRequest<LoginResponseDTO>(url, HttpMethod.Post, dto);

            if (!response.IsSuccess)
            { 
                return response;
            }

            ServiceConfig.SetAuthToken(response.Data.Token);

            return response;
        }

        public async Task<ApiResponse<LoginResponseDTO>> Login(LoginViewModel model)
        {
            var dto = MapModelToDto(model);
            var url = ServiceConfig.BaseURL + "User/Login";
            var response = await restService.SendRequest<LoginResponseDTO>(url, HttpMethod.Post, dto);

            if (!response.IsSuccess)
            {
                return response;
            }

            if (!response.Content.Contains(ServiceConfig.MFANeededCode))
            {
                ServiceConfig.SetAuthToken(response.Data.Token);
            }

            return response;
        }

        private LoginDto MapModelToDto(LoginViewModel model)
        {
            if (model == null)
                return null;

            return new LoginDto
            {
                Email = model.Username,
                Password = model.Password,
                RememberMe = model.RememberMe
            };
        }

        private MFALoginDTO MapVmToMFADto(LoginViewModel vm)
        {
            return new MFALoginDTO
            {
                Email = vm.Username,
                Password = vm.Password,
                MfaCode = vm.MfaCode,
                RememberMe = vm.RememberMe
            };
        }
    }
}
