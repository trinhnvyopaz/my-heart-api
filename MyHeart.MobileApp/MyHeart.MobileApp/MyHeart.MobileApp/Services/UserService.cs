using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MyHeart.DTO.User;
using MyHeart.MobileApp.ViewModels;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter;

namespace MyHeart.MobileApp.Services
{
    public class UserService : BaseService
    {
        private const string EndPoint = "User/";
        public int UserId { get; set; }
        public UserDTO User { get; set; }

        public async Task<UserDTO> GetCurrentAsync()
        {
            var response = await restService.SendRequest<UserDTO>($"{EndPoint}Current", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                UserId = response.Data.Id;
                User = response.Data;

                AppCenter.SetUserId($"{UserId}");

                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<bool> DoestUserHaveActiveMFA()
        {
            var response = await restService.SendRequest<string>($"{EndPoint}doesUserHaveMfa", HttpMethod.Get, null);
            return true;
        }
        public async Task<UserDetailDTO> GetUserDetailAsync()
        {
            var response = await restService.SendRequest<UserDetailDTO>($"{EndPoint}detail/{UserId}", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<UserReportDTO>> GetReports()
        {
            var response = await restService.SendRequest<IEnumerable<UserReportDTO>>($"{EndPoint}Reports/{UserId}", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }


        public async Task<IEnumerable<UserReportFileDTO>> GetReportFiles(int reportId)
        {
            string arg = $"{EndPoint}Reports/{UserId}/{reportId}";

            var response = await restService.SendRequest<IEnumerable<UserReportFileDTO>>(arg, HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> UpdateAsync(UserDetailViewModel userDetail)
        {
            var dto = MapVmToDto(userDetail);

            var response = await restService.SendRequest($"{EndPoint}Update", HttpMethod.Patch, dto);

            return response.IsSuccess;
        }

        public async Task<UserReportDTO> AddReportAsync(ReportViewModel report)
        {
            var dto = MapVmToDto(report);

            var response = await restService.SendRequest<UserReportDTO>($"{EndPoint}Reports/Add/{UserId}", HttpMethod.Post, dto);

            return response.Data;
        }
        public async Task<bool> DeleteReport(int reportId)
        {
            var response = await restService.SendRequest($"{EndPoint}Reports/Delete/{UserId}/{reportId}", HttpMethod.Delete, null);

            return response.IsSuccess;
        }
        public async Task<bool> UpdateReport(int reportId, UserReportUpdateDTO reportUpdateDTO)
        {
            var response = await restService.SendRequest($"{EndPoint}Reports/Update/{UserId}/{reportId}", HttpMethod.Patch, reportUpdateDTO);

            return response.IsSuccess;
        }

        public async Task<UserNotificationSettingsDTO> GetNotificationSettings()
        {
            var response = await restService.SendRequest<UserNotificationSettingsDTO>($"{EndPoint}NotificationSettings", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> UpdateNotificationSettings(UserNotificationSettingsViewModel vm)
        {
            var dto = MapVmToDto(vm);

            var response = await restService.SendRequest($"{EndPoint}NotificationSettings", HttpMethod.Put, dto);

            return response.IsSuccess;
        }

        public async Task<UserDTO> RegisterUser(RegisterViewModel vm)
        {
            var dto = MapVmToDto(vm);
            var response = await restService.SendRequest<UserDTO>($"{EndPoint}register", HttpMethod.Post, dto);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<LoginResponseDTO> MobileLogin(MobileLoginDTO dto)
        {
            var response = await restService.SendRequest<LoginResponseDTO>($"{EndPoint}mobileLogin", HttpMethod.Post, dto);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<bool> AuthorizeByPhone(PhoneAuthResponseDTO dto)
        {
            var response = await restService.SendRequest($"{EndPoint}loginByPhone", HttpMethod.Post, dto);

            return response.IsSuccess;

        }

        public async Task<UserFmcTokenDTO> AddFmcToken(string token)
        {
            var fmcToken = new UserFmcTokenDTO
            {
                Token = token,
                UserId = UserId
            };

            var response = await restService.SendRequest<UserFmcTokenDTO>($"{EndPoint}AddFmcToken", HttpMethod.Patch, fmcToken);

            return response.Data;
        }

        private RegisterDTO MapVmToDto(RegisterViewModel vm)
        {
            return new RegisterDTO
            {
                Email = vm.Email,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Password = vm.Password,
                PasswordAgain = vm.PasswordAgain
            };
        }

        private UserNotificationSettingsDTO MapVmToDto(UserNotificationSettingsViewModel vm)
        {
            return new UserNotificationSettingsDTO
            {
                ReplyEmailNotification = vm.ReplyEmailNotification,
                SubscriptionPreferences = vm.SubscriptionPreferences,
                TherapyNewsEmailNotification = vm.TherapyNewsEmailNotification,
                UserID = vm.UserID
            };
        }
        private UserReportDTO MapVmToDto(ReportViewModel report)
        {
            return new UserReportDTO()
            {
                Title = report.Title,
                Description = report.Description,
                LastUpdate = report.LastUpdate,
                CreationDate = report.CreationDate,
                UserId = this.UserId,
                ReportType = report.ReportType,
                Files = report.Files.Select(rfvm => new UserReportFileDTO()
                {
                    Name = rfvm.Name,
                    Extension = rfvm.Extension,
                    Content = rfvm.Content,
                }).ToList()
            };
        }
        private UserDetailDTO MapVmToDto(UserDetailViewModel vm)
        {
            return new UserDetailDTO()
            {
                Id = vm.Id,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                Phone = vm.Phone,
                Street = vm.Street,
                StreetNumber = vm.StreetNumber,
                City = vm.City,
                Zip = vm.Zip,
                PIN = vm.PIN,
                DoctorId = vm.DoctorId,
                SubscriptionId = vm.SubscriptionId,
                Subscription = vm.Subscription,
                UserId = vm.UserId,
                IsFamilyAnamnesis = vm.IsFamilyAnamnesis,
                IsPersonalAnamnesis = vm.IsPersonalAnamnesis,
                IsPharmacyAnamnesis = vm.IsPharmacyAnamnesis,
                IsAllergyAnamnesis = vm.IsAllergyAnamnesis,
                IsAbususAnamnesis = vm.IsAbususAnamnesis,
                IsSocialAnamnesis = vm.IsSocialAnamnesis,
                IsFamily_ICHS = vm.IsFamily_ICHS,
                IsFamily_ValveDefect = vm.IsFamily_ValveDefect,
                IsFamily_AtrialFibrillation = vm.IsFamily_AtrialFibrillation,
                IsFamily_SuddenDeath = vm.IsFamily_SuddenDeath,
                IsFamily_Pacemaker = vm.IsFamily_Pacemaker,
                Personal_Diseases = vm.Personal_Diseases,
                Personal_NonpharmaticId = vm.Personal_NonpharmaticId,
                Pharmacies = vm.Pharmacies,
                IsAllergy_Name = vm.IsAllergy_Name,
                IsAbusus_Alcohol = vm.IsAbusus_Alcohol,
                IsAbusus_Exsmoker = vm.IsAbusus_Exsmoker,
                IsAbusus_Smoker = vm.IsAbusus_Smoker,
                IsSocial_LivingWithPartner = vm.IsSocial_LivingWithPartner,
                IsSocial_Working = vm.IsSocial_Working,
                IsSocial_Pension = vm.IsSocial_Pension,
                IsSocial_PartialDisabilityPension = vm.IsSocial_PartialDisabilityPension,
                IsSocial_DisabilityPension = vm.IsSocial_DisabilityPension,
                InsuranceCompanyCode = vm.InsuranceCompanyCode,
                InsuranceNumber = vm.InsuranceNumber
            };
        }
    }
}
