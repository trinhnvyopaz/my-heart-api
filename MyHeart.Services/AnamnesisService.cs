using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO.User;
using MyHeart.Services.Interfaces;
using Org.BouncyCastle.Asn1.Microsoft;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services
{
    public class AnamnesisService : IAnamnesisService
    {

        private readonly MyHeartContext _myheartContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;


        public AnamnesisService(MyHeartContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _myheartContext = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        private async Task<User> GetCurrentUser()
        {
            if (_httpContextAccessor.HttpContext.User == null)
                return null;

            var userIdString = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdString, out int userId))
                return null;

            var user = await _myheartContext.Users.FindAsync(userId);

            if (user == null)
                return null;

            return user;
        }

        private async Task<User> GetUserById(int id)
        {
            return await _myheartContext.Users.FindAsync(id);
        }

        #region BelongsCheck
        public async Task<bool> DoesAnamnesisBelongToUser(int userId, int AnamnesisId)
        {
            var anamnesis = await _myheartContext.UserAnamnesis.FindAsync(AnamnesisId);

            return anamnesis != null && anamnesis.UserId == userId;
        }
        #endregion

        #region Abusus
        public async Task<UserAnamnesisDTO> GetAbususAnamnesis()
        {
            var user = await GetCurrentUser();

            var anamnesis = await _myheartContext.UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .Where(x => x.IsAbususAnamnesis)
                .FirstOrDefaultAsync();

            if (anamnesis == null)
            {
                anamnesis = new UserAnamnesis
                {
                    IsAbususAnamnesis = true,
                    UserId = user.Id
                };
                _myheartContext.Add(anamnesis);
                await _myheartContext.SaveChangesAsync();
            }

            var result = _mapper.Map<UserAnamnesis, UserAnamnesisDTO>(anamnesis);

            return result;
        }

        public async Task<UserAnamnesisDTO> GetAbususAnamnesis(int userId)
        {

            var anamnesis = await _myheartContext.UserAnamnesis
                .Where(x => x.UserId == userId)
                .Where(x => x.IsAbususAnamnesis)
                .FirstOrDefaultAsync();

            if (anamnesis == null)
            {
                anamnesis = new UserAnamnesis
                {
                    IsAbususAnamnesis = true,
                    UserId = userId
                };
                _myheartContext.Add(anamnesis);
                await _myheartContext.SaveChangesAsync();
            }

            var result = _mapper.Map<UserAnamnesis, UserAnamnesisDTO>(anamnesis);

            return result;
        }

        public async Task<UserAnamnesisDTO> SaveAbususAnamnesis(UserAnamnesisDTO model, string userName)
        {
            if (model == null)
                return null;

            var user = await GetUserById(model.UserId);

            var dbModel = await _myheartContext.UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .Where(x => x.IsAbususAnamnesis)
                .FirstOrDefaultAsync();

            if (dbModel == null)
            {
                _myheartContext.Add(new UserAnamnesis
                {
                    IsAbususAnamnesis = true,
                    IsAbusus_Alcohol = model.IsAbusus_Alcohol,
                    IsAbusus_Exsmoker = model.IsAbusus_Exsmoker,
                    IsAbusus_Smoker = model.IsAbusus_Smoker,

                    UserId = user.Id,
                    LastUpdateUser = userName
                }); ;
                await _myheartContext.SaveChangesAsync();
            }
            else
            {
                dbModel.IsAbusus_Alcohol = model.IsAbusus_Alcohol;
                dbModel.IsAbusus_Exsmoker = model.IsAbusus_Exsmoker;
                dbModel.IsAbusus_Smoker = model.IsAbusus_Smoker;
                dbModel.LastUpdateUser = userName;

                _myheartContext.Entry(dbModel).State = EntityState.Modified;
                await _myheartContext.SaveChangesAsync();
            }

            return model;
        }
        #endregion

        #region Social

        public async Task<UserAnamnesisDTO> GetSocialAnamnesis()
        {
            var user = await GetCurrentUser();

            var anamnesis = await _myheartContext.UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .Where(x => x.IsSocialAnamnesis)
                .FirstOrDefaultAsync();

            if (anamnesis == null)
            {
                anamnesis = new UserAnamnesis
                {
                    IsSocialAnamnesis = true,
                    UserId = user.Id
                };
                _myheartContext.Add(anamnesis);
                await _myheartContext.SaveChangesAsync();
            }

            var result = _mapper.Map<UserAnamnesis, UserAnamnesisDTO>(anamnesis);

            return result;
        }

        public async Task<UserAnamnesisDTO> GetSocialAnamnesis(int id)
        {
            var user = await GetUserById(id);

            var anamnesis = await _myheartContext.UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .Where(x => x.IsSocialAnamnesis)
                .FirstOrDefaultAsync();

            if (anamnesis == null)
            {
                anamnesis = new UserAnamnesis
                {
                    IsSocialAnamnesis = true,
                    UserId = user.Id
                };
                _myheartContext.Add(anamnesis);
                await _myheartContext.SaveChangesAsync();
            }

            var result = _mapper.Map<UserAnamnesis, UserAnamnesisDTO>(anamnesis);

            return result;
        }
        public async Task<UserAnamnesisDTO> GetGeneralAnamnesis()
        {
            var user = await GetCurrentUser();

            var anamnesis = await _myheartContext.UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .Where(x => x.IsGeneralAnamnesis)
                .FirstOrDefaultAsync();

            if (anamnesis == null)
            {
                anamnesis = new UserAnamnesis
                {
                    IsGeneralAnamnesis = true,
                    UserId = user.Id
                };
                _myheartContext.Add(anamnesis);
                await _myheartContext.SaveChangesAsync();
            }

            var result = _mapper.Map<UserAnamnesis, UserAnamnesisDTO>(anamnesis);

            return result;
        }
        public async Task<UserAnamnesisDTO> SaveGeneralAnamnesis(UserAnamnesisDTO model, string userName)
        {
            if (model == null)
                return null;

            var user = await GetUserById(model.UserId);

            var dbModel = await _myheartContext.UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .Where(x => x.IsGeneralAnamnesis)
                .FirstOrDefaultAsync();

            if (dbModel == null)
            {
                _myheartContext.Add(new UserAnamnesis
                {
                    IsGeneralAnamnesis = true,
                    IsGeneral_PhysicalActivityFrequencyType = model.IsGeneral_PhysicalActivityFrequencyType,

                    UserId = user.Id,
                    LastUpdateUser = userName
                });
                await _myheartContext.SaveChangesAsync();
            }
            else
            {
                dbModel.IsGeneral_PhysicalActivityFrequencyType = model.IsGeneral_PhysicalActivityFrequencyType;
                dbModel.LastUpdateUser = userName;

                _myheartContext.Entry(dbModel).State = EntityState.Modified;
                await _myheartContext.SaveChangesAsync();
            }

            return model;
        }
        public async Task<UserAnamnesisDTO> SaveSocialAnamnesis(UserAnamnesisDTO model, string userName)
        {
            if (model == null)
                return null;

            var user = await GetUserById(model.UserId);

            var dbModel = await _myheartContext.UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .Where(x => x.IsSocialAnamnesis)
                .FirstOrDefaultAsync();

            if (dbModel == null)
            {
                _myheartContext.Add(new UserAnamnesis
                {
                    IsSocialAnamnesis = true,
                    IsSocial_DisabilityPension = model.IsSocial_DisabilityPension,
                    IsSocial_LivingWithPartner = model.IsSocial_LivingWithPartner,
                    IsSocial_PartialDisabilityPension = model.IsSocial_PartialDisabilityPension,
                    IsSocial_Pension = model.IsSocial_Pension,
                    IsSocial_Working = model.IsSocial_Working,

                    UserId = user.Id,
                    LastUpdateUser = userName
                });
                await _myheartContext.SaveChangesAsync();
            }
            else
            {
                dbModel.IsSocial_DisabilityPension = model.IsSocial_DisabilityPension;
                dbModel.IsSocial_LivingWithPartner = model.IsSocial_LivingWithPartner;
                dbModel.IsSocial_PartialDisabilityPension = model.IsSocial_PartialDisabilityPension;
                dbModel.IsSocial_Pension = model.IsSocial_Pension;
                dbModel.IsSocial_Working = model.IsSocial_Working;
                dbModel.LastUpdateUser = userName;

                _myheartContext.Entry(dbModel).State = EntityState.Modified;
                await _myheartContext.SaveChangesAsync();
            }

            return model;
        }

        #endregion

        #region Family

        public async Task<UserAnamnesisDTO> GetFamilyAnamnesis()
        {
            var user = await GetCurrentUser();

            var anamnesis = await _myheartContext.UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .Where(x => x.IsFamilyAnamnesis)
                .FirstOrDefaultAsync();

            if (anamnesis == null)
            {
                anamnesis = new UserAnamnesis
                {
                    IsFamilyAnamnesis = true,
                    UserId = user.Id
                };
                _myheartContext.Add(anamnesis);
                await _myheartContext.SaveChangesAsync();
            }

            var result = _mapper.Map<UserAnamnesis, UserAnamnesisDTO>(anamnesis);

            return result;
        }

        public async Task<UserAnamnesisDTO> GetFamilyAnamnesis(int id)
        {
            var user = await GetUserById(id);

            var anamnesis = await _myheartContext.UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .Where(x => x.IsFamilyAnamnesis)
                .FirstOrDefaultAsync();

            if (anamnesis == null)
            {
                anamnesis = new UserAnamnesis
                {
                    IsFamilyAnamnesis = true,
                    UserId = user.Id
                };
                _myheartContext.Add(anamnesis);
                await _myheartContext.SaveChangesAsync();
            }

            var result = _mapper.Map<UserAnamnesis, UserAnamnesisDTO>(anamnesis);

            return result;
        }

        public async Task<UserAnamnesisDTO> SaveFamilyAnamnesis(UserAnamnesisDTO model, string userName)
        {
            if (model == null)
                return null;

            var user = await GetUserById(model.UserId);

            var dbModel = await _myheartContext.UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .Where(x => x.IsFamilyAnamnesis)
                .FirstOrDefaultAsync();

            if (dbModel == null)
            {
                _myheartContext.Add(new UserAnamnesis
                {
                    IsFamilyAnamnesis = true,
                    IsFamily_AtrialFibrillation = model.IsFamily_AtrialFibrillation,
                    IsFamily_ICHS = model.IsFamily_ICHS,
                    IsFamily_Pacemaker = model.IsFamily_Pacemaker,
                    IsFamily_SuddenDeath = model.IsFamily_SuddenDeath,
                    IsFamily_ValveDefect = model.IsFamily_ValveDefect,
                    IsFamily_HeartAttack = model.IsFamily_HeartAttack,

                    UserId = user.Id,
                    LastUpdateUser = userName
                });
                await _myheartContext.SaveChangesAsync();
            }
            else
            {
                dbModel.IsFamily_AtrialFibrillation = model.IsFamily_AtrialFibrillation;
                dbModel.IsFamily_ICHS = model.IsFamily_ICHS;
                dbModel.IsFamily_Pacemaker = model.IsFamily_Pacemaker;
                dbModel.IsFamily_SuddenDeath = model.IsFamily_SuddenDeath;
                dbModel.IsFamily_ValveDefect = model.IsFamily_ValveDefect;
                dbModel.IsFamily_HeartAttack = model.IsFamily_HeartAttack;
                dbModel.LastUpdateUser = userName;

                await _myheartContext.SaveChangesAsync();
            }

            return model;
        }

        #endregion

        public async Task<IEnumerable<UserAnamnesisDTO>> GetPersonalAnamnesis()
        {
            var user = await GetCurrentUser();

            var anamneses = await _myheartContext
                .UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .Where(x => x.IsPersonalAnamnesis)
                .ToListAsync();

            var result = _mapper.Map<IEnumerable<UserAnamnesis>, IEnumerable<UserAnamnesisDTO>>(anamneses);

            return result;
        }

        public async Task<IEnumerable<UserAnamnesisDTO>> GetPersonalAnamnesisByType(PersonalAnamnesisType type)
        {
            var user = await GetCurrentUser();

            var anamneses = await _myheartContext
                .UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .Where(x => x.IsPersonalAnamnesis)
                .Where(x => x.IsPersonal_Type == (int)type)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var result = _mapper.Map<IEnumerable<UserAnamnesis>, IEnumerable<UserAnamnesisDTO>>(anamneses);

            return result;
        }

        public async Task<IEnumerable<UserAnamnesisDTO>> GetPersonalAnamnesis(int id)
        {
            var user = await GetUserById(id);

            var anamneses = await _myheartContext
                .UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .Where(x => x.IsPersonalAnamnesis)
                .ToListAsync();

            var result = _mapper.Map<IEnumerable<UserAnamnesis>, IEnumerable<UserAnamnesisDTO>>(anamneses);

            return result;
        }

        public async Task<UserAnamnesisDTO> SavePersonalAnamnesis(UserAnamnesisDTO model, string userName)
        {
            if (model == null)
                return null;

            if(model.IsPersonal_CreatorType != PersonalAnamnesisCreatorType.User)
            {
                bool isExported = await _myheartContext.UserAnamnesis
                    .Where(u => u.IsPersonal_Type == model.IsPersonal_Type)
                    .Where(u => u.IsPersonal_Date.Date == model.IsPersonal_Date.Date)
                    .Where(u => u.IsPersonal_CreatorType != PersonalAnamnesisCreatorType.User)
                    .AnyAsync();

                if (isExported)
                    return null;
            }

            var newUserAnamnesis = new UserAnamnesis
            {
                IsPersonalAnamnesis = true,
                IsPersonal_Date = model.IsPersonal_Date.ToLocalTime(),
                IsPersonal_Type = model.IsPersonal_Type,
                IsPersonal_Value = model.IsPersonal_Value,
                IsPersonal_CreatorType = model.IsPersonal_CreatorType,

                UserId = model.UserId,
                LastUpdateUser = userName
            };

            _myheartContext.Add(newUserAnamnesis);
            await _myheartContext.SaveChangesAsync();

            return _mapper.Map<UserAnamnesisDTO>(newUserAnamnesis);
        }
        public async Task<UserAnamnesisDTO> UpdatePersonalAnamnesis(UserAnamnesisDTO model, string userName)
        {
            if (model == null)
                return null;

            var dbAnamnesis = await _myheartContext.UserAnamnesis.FindAsync(model.Id);
            if (dbAnamnesis == null)
                return null;

            dbAnamnesis.IsPersonal_Date = model.IsPersonal_Date.ToLocalTime();
            dbAnamnesis.IsPersonal_Type = model.IsPersonal_Type;
            dbAnamnesis.IsPersonal_Value = model.IsPersonal_Value;

            dbAnamnesis.UserId = model.UserId;
            dbAnamnesis.LastUpdateUser = userName;

            await _myheartContext.SaveChangesAsync();

            return _mapper.Map<UserAnamnesisDTO>(dbAnamnesis);
        }

        public async Task<bool> DeletePersonalAnamnesis(int id)
        {
            var dbModel = await _myheartContext.UserAnamnesis
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dbModel == null)
                return false;

            var user = await GetCurrentUser();

            if (user == null)
                return false;

            if (dbModel.UserId != user.Id && user.UserType == UType.Patient)
                return false;

            _myheartContext.Remove(dbModel);
            await _myheartContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<UserAnamnesisDTO>> GetPersonalPharmacy()
        {
            var user = await GetCurrentUser();

            var result = await _myheartContext
                .UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .Where(x => x.IsPharmacyAnamnesis)
                .ToListAsync();

            var anamneses = _mapper.Map<IEnumerable<UserAnamnesis>, IEnumerable<UserAnamnesisDTO>>(result);

            return anamneses;
        }

        public async Task<IEnumerable<UserAnamnesisDTO>> GetPersonalPharmacy(int id)
        {
            var user = await GetUserById(id);

            var result = await _myheartContext
                .UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .Where(x => x.IsPharmacyAnamnesis)
                .ToListAsync();

            var anamneses = _mapper.Map<IEnumerable<UserAnamnesis>, IEnumerable<UserAnamnesisDTO>>(result);

            return anamneses;
        }

        public async Task<UserAnamnesisDTO> SavePharmacyAnamnesis(UserAnamnesisDTO model, string userName)
        {
            //var existingData = await _myheartContext.UserAnamnesis
            //    .Where(x => x.UserId == user.Id)
            //    .Where(x => x.IsPharmacyAnamnesis)
            //    .ToListAsync();

            //_myheartContext.RemoveRange(existingData);
            //await _myheartContext.SaveChangesAsync();

            if (model.Id > 0)
            {
                var dbModel = await _myheartContext.UserAnamnesis
                    .FirstOrDefaultAsync(x => x.Id == model.Id);

                if (dbModel == null)
                    return null;

                dbModel.IsPharmacy_MorningDose = model.IsPharmacy_MorningDose;
                dbModel.IsPharmacy_AfternoonDose = model.IsPharmacy_AfternoonDose;
                dbModel.IsPharmacy_EveningDose = model.IsPharmacy_EveningDose;
                dbModel.IsPharmacy_Note = model.IsPharmacy_Note;
                dbModel.CreatedAt = model.CreatedAt;

                _myheartContext.Entry(dbModel).State = EntityState.Modified;
                await _myheartContext.SaveChangesAsync();

                return model;
            }

            else
            {
                var dbModel = new UserAnamnesis
                {
                    UserId = model.UserId,
                    IsPharmacyAnamnesis = true,
                    IsPharmacy_PharmacyId = model.IsPharmacy_PharmacyId,
                    IsPharmacy_Name = model.IsPharmacy_Name,
                    IsPharmacy_Dose = model.IsPharmacy_Dose,
                    IsPharmacy_MorningDose = model.IsPharmacy_MorningDose,
                    IsPharmacy_AfternoonDose = model.IsPharmacy_AfternoonDose,
                    IsPharmacy_EveningDose = model.IsPharmacy_EveningDose,
                    IsPharmacy_Note = model.IsPharmacy_Note,
                    LastUpdateUser = userName
                };

                _myheartContext.UserAnamnesis
                    .Add(dbModel);
                await _myheartContext.SaveChangesAsync();

                return _mapper.Map<UserAnamnesisDTO>(dbModel);
            }
        }

        public async Task<bool> DeletePharmacyAnamnesis(int id)
        {
            var dbModel = await _myheartContext.UserAnamnesis
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            _myheartContext.UserAnamnesis
                    .Remove(dbModel);
            await _myheartContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<PharmacyDTO>> GetPharmacyByNameReg(string nameReg)
        {
            if (string.IsNullOrEmpty(nameReg))
                return null;

            var result = await _myheartContext.Pharmacy
                .Where(x => x.NameReg.ToLower().Contains(nameReg.ToLower()))
                .ToListAsync();

            return _mapper.Map<IEnumerable<Pharmacy>, IEnumerable<PharmacyDTO>>(result);
        }
        public async Task<IEnumerable<PharmacyDTO>> GetPharmacyByNameRegStrength(string nameReg, string strength)
        {
            bool nameRegEmpty = string.IsNullOrEmpty(nameReg) || nameReg == "null";
            bool strengthEmpty = string.IsNullOrEmpty(strength) || strength == "null";

            if (nameRegEmpty && strengthEmpty)
                return null;

            IQueryable<Pharmacy> query = _myheartContext.Pharmacy;

            if (!nameRegEmpty)
            {
                query = query.Where(x => x.NameReg.ToLower().Contains(nameReg.ToLower()));
            }
            if (!strengthEmpty)
            {
                query = query.Where(x => x.Strength.ToLower().Contains(strength.ToLower()));
            }

            var result = await query.ToListAsync();

            return _mapper.Map<IEnumerable<Pharmacy>, IEnumerable<PharmacyDTO>>(result);
        }

        public async Task<IEnumerable<UserAnamnesisDTO>> GetAllergyAnamneses()
        {
            var user = await GetCurrentUser();

            return await GetAllergyAnamneses(user);
        }

        public async Task<IEnumerable<UserAnamnesisDTO>> GetAllergyAnamneses(int id)
        {
            var user = await GetUserById(id);

            return await GetAllergyAnamneses(user);
        }

        public async Task<IEnumerable<UserAnamnesisDTO>> GetAllergyAnamneses(User user)
        {
            if (user == null)
                return null;

            var result = await _myheartContext.UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .Where(x => x.IsAllergyAnamnesis)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserAnamnesis>, IEnumerable<UserAnamnesisDTO>>(result);
        }

        public async Task<UserAnamnesisDTO> SaveAllergyAnamneses(UserAnamnesisDTO model, string userName)
        {
            if (model == null)
                return null;

            var user = await GetUserById(model.UserId);

            if (user == null)
                return null;

            if (model.Id == 0)
            {
                var dbAllergy = new UserAnamnesis
                {
                    UserId = user.Id,
                    IsAllergyAnamnesis = true,
                    IsAllergy_Name = model.IsAllergy_Name,
                    LastUpdateUser = userName
                };

                _myheartContext.Add(dbAllergy);

                await _myheartContext.SaveChangesAsync();

                return _mapper.Map<UserAnamnesis, UserAnamnesisDTO>(dbAllergy);
            }
            else
            {
                var dbAllergy = await _myheartContext.UserAnamnesis.FirstOrDefaultAsync(x => x.Id == model.Id);

                if (dbAllergy == null)
                    return null;

                dbAllergy.IsAllergy_Name = model.IsAllergy_Name;

                await _myheartContext.SaveChangesAsync();

                return _mapper.Map<UserAnamnesis, UserAnamnesisDTO>(dbAllergy);
            }
        }

        public async Task<bool> DeleteAllergyAnamnesis(int id)
        {
            var dbModel = await _myheartContext.UserAnamnesis
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (dbModel == null)
                return false;

            var user = await GetCurrentUser();

            if (dbModel.UserId != user.Id && user.UserType == UType.Patient)
                return false;

            _myheartContext.Remove(dbModel);
            await _myheartContext.SaveChangesAsync();

            return true;
        }


        #region UserDiseases

        public async Task<IEnumerable<DiseaseDTO>> GetDiseasesByName(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
                return null;

            var result = await _myheartContext.Disease
                .Where(x => x.Name.ToLower().Contains(searchString.ToLower()))
                .ToListAsync();

            return _mapper.Map<IEnumerable<Disease>, IEnumerable<DiseaseDTO>>(result);
        }

        public async Task<IEnumerable<DiseaseDTO>> GetAllDieseases()
        {
            var result = await _myheartContext.Disease
                .ToListAsync();

            return _mapper.Map<IEnumerable<Disease>, IEnumerable<DiseaseDTO>>(result);
        }

        public async Task<DataTableResponse<DiseaseDTO>> GetDiseases(DataTableRequest request)
        {
            var diseases = await _myheartContext.Disease
                .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()))
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var totalCount = await _myheartContext.Disease
                .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()))
                .CountAsync();

            return new DataTableResponse<DiseaseDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Data = _mapper.Map<IEnumerable<Disease>, IEnumerable<DiseaseDTO>>(diseases).ToList(),
                TotalCount = totalCount
            };
        }

        public async Task<IEnumerable<UserDiseaseDto>> GetDiseaseAnamneses()
        {
            var user = await GetCurrentUser();

            return await GetDiseaseAnamneses(user);
        }

        public async Task<IEnumerable<UserDiseaseDto>> GetAllDiseaseAnamneses()
        {
            var result = await _myheartContext.UserDiseases
                .Include(x => x.Disease)
                .Include(x => x.User)
                    .ThenInclude(x => x.UserDetail)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserDisease>, IEnumerable<UserDiseaseDto>>(result);
        }

        public async Task<IEnumerable<UserDiseaseDto>> GetDiseaseAnamneses(int id)
        {
            var user = await GetUserById(id);

            return await GetDiseaseAnamneses(user);
        }

        private async Task<IEnumerable<UserDiseaseDto>> GetDiseaseAnamneses(User user)
        {
            if (user == null)
                return null;

            var result = await _myheartContext.UserDiseases
                .Include(x => x.Disease)
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserDisease>, IEnumerable<UserDiseaseDto>>(result);
        }

        public async  Task<UserDiseaseDto> GetDiseaseAnamnesisById(int id)
        {
            var result = await _myheartContext.UserDiseases
                             .Include(x => x.Disease)
                             .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<UserDiseaseDto>(result);
        }

        public async Task<UserDiseaseDto> SaveDiseaseAnamnesis(UserDiseaseDto model, string userName)
        {
            if (model == null)
                return null;

            var user = await GetUserById(model.UserId);

            if (user == null)
                return null;

            if (model.Id == 0)
            {
                var dbModel = new UserDisease
                {
                    DiseaseId = model.DiseaseId,
                    StartDateString = model.StartDateString,
                    Note = model.Note,
                    StartDate = model.StartDate,
                    UserId = user.Id,
                    LastUpdateUser = userName
                };

                _myheartContext.UserDiseases
                    .Add(dbModel);

                await _myheartContext.SaveChangesAsync();

                return await GetDiseaseAnamnesisById(dbModel.Id);
            }

            else
            {
                var dbModel = await _myheartContext.UserDiseases
                    .FirstOrDefaultAsync(x => x.Id == model.Id);

                if (dbModel == null)
                    return null;

                dbModel.Note = model.Note;
                dbModel.StartDate = model.StartDate;
                dbModel.StartDateString = model.StartDateString;
                dbModel.LastUpdateUser = userName;

                _myheartContext.Entry(dbModel).State = EntityState.Modified;
                await _myheartContext.SaveChangesAsync();

                return await GetDiseaseAnamnesisById(dbModel.Id);
            }


        }

        public async Task<bool> DeleteDiseaseAnamnesis(int id)
        {
            if (id < 1)
                return false;

            var user = await GetCurrentUser();

            var dbModel = await _myheartContext.UserDiseases
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dbModel == null)
                return false;

            if (dbModel.UserId != user.Id && user.UserType == UType.Patient)
                return false;

            _myheartContext.Remove(dbModel);
            await _myheartContext.SaveChangesAsync();

            return true;
        }

        #endregion

        public async Task<IEnumerable<UserAnamnesisDTO>> GetAllAnamneses()
        {
            var user = await GetCurrentUser();

            var result = await _myheartContext.UserAnamnesis
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserAnamnesis>, IEnumerable<UserAnamnesisDTO>>(result);
        }
        public async Task<byte[]> GetPersonalAnamnesisCsv()
        {
            var list = await GetPersonalAnamnesis();

            //var list = _mapper.Map < IEnumerable<UserAnamnesis>, IEnumerable<UserAnamnesisDTO>>(await _myheartContext.UserAnamnesis.ToListAsync());

            byte[] result;

            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                    {
                        csvWriter.WriteRecords(list);
                        streamWriter.Flush();
                        result = memoryStream.ToArray();
                    }
                }
            }

            return result;

        }

        public async Task<IEnumerable<UserAnamnesisDTO>> GetAllPharmacyAnamnesis()
        {
            var pharmacies = await _myheartContext.UserAnamnesis
                .Where(x => x.IsPharmacyAnamnesis)
                .ToListAsync();

            return pharmacies.Select(_mapper.Map<UserAnamnesisDTO>);
        }
    }
}
