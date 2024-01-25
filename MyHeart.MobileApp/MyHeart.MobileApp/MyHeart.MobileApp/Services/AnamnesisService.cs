using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO.User;
using MyHeart.MobileApp.Enums;
using MyHeart.MobileApp.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyHeart.MobileApp.Services
{
    public class AnamnesisService : BaseService
    {
        private const string EndPoint = "Anamnesis/";
        private UserService userService;

        public AnamnesisService()
        {
            userService = DependencyService.Resolve<UserService>();
        }

        public async Task<IEnumerable<UserAnamnesisDTO>> GetAllAnamneses()
        {
            var response = await restService.SendRequest<IEnumerable<UserAnamnesisDTO>>($"{EndPoint}getAll", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<IEnumerable<UserAnamnesisDTO>> GetPharmacyAnamnesis()
        {
            var response = await restService.SendRequest<IEnumerable<UserAnamnesisDTO>>($"{EndPoint}getPharmacyAnamnesis", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<UserAnamnesisDTO> SavePersonalAsync(PersonalRecordViewModel record)
        {
            var dto = new UserAnamnesisDTO()
            {
                UserId = userService.UserId,
                IsPersonal_Value = record.Value,
                IsPersonal_Date = record.Date.Date + record.Time,
                IsPersonal_Type = (int)record.Type,
                IsPersonal_CreatorType = record.IsPersonal_CreatorType
            };

            var response = await restService.SendRequest<UserAnamnesisDTO>($"{EndPoint}savePersonalAnamnesis", HttpMethod.Post, dto);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<UserAnamnesisDTO> UpdatePersonalAsync(PersonalRecordViewModel record)
        {
            var dto = new UserAnamnesisDTO()
            {
                Id = record.Id,
                UserId = userService.UserId,
                IsPersonal_Value = record.Value,
                IsPersonal_Date = record.Date.Date + record.Time,
                IsPersonal_Type = (int)record.Type
            };

            var response = await restService.SendRequest<UserAnamnesisDTO>($"{EndPoint}updatePersonalAnamnesis/{record.Id}", HttpMethod.Put, dto);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeletePharmacyAnamnesis(int id)
        {
            var response = await restService.SendRequest($"{EndPoint}deletePharmacyAnamnesis/{id}", HttpMethod.Delete, null);

            return response.IsSuccess;
        }
        public async Task<UserAnamnesisDTO> SavePharmacyAnamnesis(UserAnamnesisViewModel vm)
        {
            var dto = MapVmToDto(vm);
            var response = await restService.SendRequest<UserAnamnesisDTO>($"{EndPoint}savePharmacyAnamnesis", HttpMethod.Post, dto);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<IEnumerable<PharmacyDTO>> GetPharmacyByNameReg(string query)
        {
            var response = await restService.SendRequest<IEnumerable<PharmacyDTO>>($"{EndPoint}getPharmacyByNameReg/{query}", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<UserAnamnesisDTO> SaveFamilyAnamnesis(UserAnamnesisViewModel vm)
        {
            vm.UserId = userService.UserId;
            var dto = MapVmToDto(vm);
            var response = await restService.SendRequest<UserAnamnesisDTO>($"{EndPoint}saveFamilyAnamnesis", HttpMethod.Post, dto);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<UserAnamnesisDTO> SaveSocialAnamnesis(UserAnamnesisViewModel vm)
        {
            vm.UserId = userService.UserId;
            var dto = MapVmToDto(vm);
            var response = await restService.SendRequest<UserAnamnesisDTO>($"{EndPoint}saveSocialAnamnesis", HttpMethod.Post, dto);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<UserAnamnesisDTO> GetFamilyAsync()
        {
            var response = await restService.SendRequest<UserAnamnesisDTO>($"{EndPoint}getFamilyAnamneses", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<IEnumerable<UserAnamnesisDTO>> GetPersonalAsync()
        {
            var response = await restService.SendRequest<IEnumerable<UserAnamnesisDTO>>($"{EndPoint}getPersonalAnamneses", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<UserAnamnesisDTO>> GetPersonalByTypeAsync(AnamnesisType type)
        {
            var url = $"{EndPoint}getPersonalAnamnesesByType/{(int)type}";
            var response = await restService.SendRequest<IEnumerable<UserAnamnesisDTO>>(url, HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<UserAnamnesisDTO> SavePersonalAsync(UserAnamnesisViewModel vm)
        {
            var dto = MapVmToDto(vm);
            var response = await restService.SendRequest<UserAnamnesisDTO>($"{EndPoint}savePersonalAnamnesis", HttpMethod.Post, dto);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<UserAnamnesisDTO> GetGeneralAsync()
        {
            var response = await restService.SendRequest<UserAnamnesisDTO>($"{EndPoint}getGeneralAnamneses", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<UserAnamnesisDTO> SaveGeneralAsync(UserAnamnesisViewModel vm)
        {
            vm.UserId = userService.UserId;
            var dto = MapVmToDto(vm);
            var response = await restService.SendRequest<UserAnamnesisDTO>($"{EndPoint}saveGeneralAnamnesis", HttpMethod.Post, dto);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<UserAnamnesisDTO> GetAbususAsync()
        {
            var response = await restService.SendRequest<UserAnamnesisDTO>($"{EndPoint}getAbususAnamneses", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<UserAnamnesisDTO> SaveAbususAsync(UserAnamnesisViewModel vm)
        {
            vm.UserId = userService.UserId;
            var dto = MapVmToDto(vm);
            var response = await restService.SendRequest<UserAnamnesisDTO>($"{EndPoint}saveAbususAnamnesis", HttpMethod.Post, dto);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<IEnumerable<UserAnamnesisDTO>> GetAllergyAsync()
        {
            var response = await restService.SendRequest<IEnumerable<UserAnamnesisDTO>>($"{EndPoint}getAllergyAnamneses", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<UserAnamnesisDTO> SaveAllergyAnamnesis(UserAnamnesisViewModel vm)
        {
            var dto = MapVmToDto(vm);
            var response = await restService.SendRequest<UserAnamnesisDTO>($"{EndPoint}saveAllergyAnamnesis", HttpMethod.Post, dto);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<bool> DeleteAllergyAsync(int Id)
        {
            var response = await restService.SendRequest<UserAnamnesisDTO>($"{EndPoint}deleteAllergyAnamnesis/{Id}", HttpMethod.Delete, null);

            return response.IsSuccess;
        }
        public async Task<IEnumerable<UserDiseaseDto>> GetDiseaseAnamneses()
        {
            var response = await restService.SendRequest<IEnumerable<UserDiseaseDto>>($"{EndPoint}getDiseaseAnamneses", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteDiseaseAnamnesis(int id)
        {
            var response = await restService.SendRequest($"{EndPoint}deleteDiseaseAnamnesis/{id}", HttpMethod.Delete, null);

            return response.IsSuccess;
        }
        public async Task<IEnumerable<DiseaseDTO>> GetDiseasesByName(string query)
        {
            var response = await restService.SendRequest<IEnumerable<DiseaseDTO>>($"{EndPoint}getDiseasesByName/{query}", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<IEnumerable<DiseaseDTO>> GetAllDiseases()
        {
            var response = await restService.SendRequest<IEnumerable<DiseaseDTO>>($"{EndPoint}getAllDiseases", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<UserDiseaseDto> SaveDiseaseAnamnesis(UserDiseaseViewModel vm)
        {
            var dto = MapVmToDto(vm);
            var response = await restService.SendRequest<UserDiseaseDto>($"{EndPoint}saveDiseaseAnamnesis", HttpMethod.Post, dto);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public async Task<UserAnamnesisDTO> GetSocialAsync()
        {
            var response = await restService.SendRequest<UserAnamnesisDTO>($"{EndPoint}getSocialAnamneses", HttpMethod.Get, null);

            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeletePersonalAnamnesis(int id)
        {
            var response = await restService.SendRequest($"{EndPoint}deletePersonalAnamnesis/{id}", HttpMethod.Delete, null);
            if (response.IsSuccess)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private UserAnamnesisDTO MapVmToDto(UserAnamnesisViewModel vm)
        {
            return new UserAnamnesisDTO
            {
                Id = vm.Id,
                UserId = vm.UserId,
                IsPharmacy_PharmacyId = vm.IsPharmacy_PharmacyId,
                IsPharmacy_Name = vm.IsPharmacy_Name,
                IsPharmacy_Dose = vm.IsPharmacy_Dose,
                IsPharmacy_MorningDose = vm.IsPharmacy_MorningDose,
                IsPharmacy_AfternoonDose = vm.IsPharmacy_AfternoonDose,
                IsPharmacy_EveningDose = vm.IsPharmacy_EveningDose,
                IsPharmacy_Note = vm.IsPharmacy_Note,
                IsAbusus_Smoker = vm.IsAbusus_Smoker,
                IsAllergyAnamnesis = vm.IsAllergyAnamnesis,
                IsAllergy_Name = vm.IsAllergy_Name,
                IsFamily_HeartAttack = vm.IsFamily_HeartAttack,
                IsPersonalAnamnesis = vm.IsPersonalAnamnesis,
                IsPersonal_Type = vm.IsPersonal_Type,
                IsPersonal_Date = vm.IsPersonal_Date,
                IsPersonal_Value = vm.IsPersonal_Value,
                IsGeneral_PhysicalActivityFrequencyType = vm.IsGeneral_PhysicalActivityFrequencyType,
                IsFamilyAnamnesis = vm.IsFamilyAnamnesis,
                IsFamily_AtrialFibrillation = vm.IsFamily_AtrialFibrillation,
                IsFamily_ICHS = vm.IsFamily_ICHS,
                IsFamily_Pacemaker = vm.IsFamily_Pacemaker,
                IsFamily_SuddenDeath = vm.IsFamily_SuddenDeath,
                IsFamily_ValveDefect = vm.IsFamily_ValveDefect,
                IsAbususAnamnesis = vm.IsAbususAnamnesis,
                IsAbusus_Alcohol = vm.IsAbusus_Alcohol,
                IsAbusus_Exsmoker = vm.IsAbusus_Exsmoker,
                IsSocialAnamnesis = vm.IsSocialAnamnesis,
                IsSocial_DisabilityPension = vm.IsSocial_DisabilityPension,
                IsSocial_LivingWithPartner = vm.IsSocial_LivingWithPartner,
                IsSocial_PartialDisabilityPension = vm.IsSocial_PartialDisabilityPension,
                IsSocial_Pension = vm.IsSocial_Pension,
                IsSocial_Working = vm.IsSocial_Working,
                CreatedAt = vm.CreatedAt
            };
        }

        private UserDiseaseDto MapVmToDto(UserDiseaseViewModel vm)
        {
            return new UserDiseaseDto
            {
                Id = vm.Id,
                UserId = vm.UserId,
                Disease = vm.Disease,
                DiseaseId = vm.DiseaseId,
                Note = vm.Note,
                StartDate = vm.StartDate,
                StartDateString = vm.StartDateString,
                User = vm.User
            };
        }
    }
}
