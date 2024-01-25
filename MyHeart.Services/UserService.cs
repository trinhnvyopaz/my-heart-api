using AutoMapper;
using MyHeart.Core.Helpers;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.Services.Interfaces;
using MyHeart.DTO.User;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Utilities;
using MyHeart.DTO;
using MyHeart.DTO.Questions;
using Microsoft.Extensions.Options;

namespace MyHeart.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MyHeartContext _myheartContext;
        private readonly IConfiguration _configuration;

        private readonly IEmailService _emailService;
        private readonly SecuritySettings _securitySettings;
        private readonly IPaymentGatewayProvider _paymentGatewayProvider;

        public UserService(
            IHttpContextAccessor httpContextAccessor,
            MyHeartContext gradologyContext,
            IMapper mapper,
            IConfiguration configuration,
            IEmailService emailService,
            IOptions<SecuritySettings> securitySettings,
            IPaymentGatewayProvider paymentGatewayProvider
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _myheartContext = gradologyContext;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
            _securitySettings = securitySettings.Value;
            _paymentGatewayProvider = paymentGatewayProvider;
        }

        public async Task<UserDTO> CurrentUserAsync()
        {
            if (_httpContextAccessor.HttpContext.User == null)
                return null;

            var userIdString = _httpContextAccessor.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;

            if (!int.TryParse(userIdString, out int userId))
                return null;

            var user = await _myheartContext.Users
                .Where(x => x.Id == userId)
                .Select(
                    u =>
                        new UserDTO
                        {
                            Id = u.Id,
                            UserType = u.UserType,
                            UserTypes = string.Join(",", u.Roles.Select(x => (int)x.RoleType)),
                            Email = u.Email,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            LastUpdateDate = u.LastUpdateDate,
                            LastUpdateUser = u.LastUpdateUser
                        }
                )
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<UserDTO> UserByIdAsync(int id)
        {
            return _mapper.Map<User, UserDTO>(await DbUserByIdAsync(id));
        }

        public async Task<DataTableResponse<UserDTO>> GetUsersTable(GroupedDataTableRequest request)
        {
            List<UserDTO> users;
            int totalCount;
            IQueryable<User> filtered;
            var groups = request.Groups.Select(g => (UType)g).ToList();

            if (!string.IsNullOrEmpty(request.Filter))
            {
                var usersName = request.Filter.Trim().Split(" ");

                if (usersName.Length == 1)
                {
                    filtered = _myheartContext.Users.Where(
                        x =>
                            x.DeleteDate == null
                            && (
                                x.FirstName.ToLower().Contains(usersName[0].ToLower())
                                || x.LastName.ToLower().Contains(usersName[0].ToLower())
                                || x.Email.ToLower().Contains(usersName[0].ToLower())
                            )
                            && (!groups.Any() || x.Roles.Any(y => groups.Contains(y.RoleType)))
                    );
                }
                else if (usersName.Length == 2)
                {
                    filtered = _myheartContext.Users.Where(
                        x =>
                            x.DeleteDate == null
                            && (
                                x.FirstName.ToLower().Contains(usersName[0].ToLower())
                                && x.LastName.ToLower().Contains(usersName[1].ToLower())
                            )
                            && (!groups.Any() || x.Roles.Any(y => groups.Contains(y.RoleType)))
                    );
                }
                else
                {
                    filtered = _myheartContext.Users.AsQueryable().Where(x => x.Id == -1);
                }
            }
            else
            {
                filtered = _myheartContext.Users.Where(
                    x =>
                        x.DeleteDate == null
                        && (!groups.Any() || x.Roles.Any(y => groups.Contains(y.RoleType)))
                );
            }

            if (request.Includes != null && request.Includes.Count > 0)
            {
                foreach (var include in request.Includes)
                {
                    filtered.Include(include);
                }
            }

            users = await filtered
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(
                    u =>
                        new UserDTO
                        {
                            Id = u.Id,
                            UserType = u.UserType,
                            UserTypes = string.Join(",", u.Roles.Select(x => (int)x.RoleType)),
                            Email = u.Email,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            LastUpdateDate = u.LastUpdateDate,
                            LastUpdateUser = u.LastUpdateUser
                        }
                )
                .ToListAsync();

            totalCount = await filtered.CountAsync();

            return new DataTableResponse<UserDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = users
            };
        }

        public async Task<bool> EmailConfirmedById(int id)
        {
            var user = await DbUserByIdAsync(id);

            return user?.EmailComfirmed ?? false;
        }

        public async Task<bool> MFAConfirmedById(int id)
        {
            var user = await DbUserByIdAsync(id);

            return user?.MFAConfirmed ?? false;
        }

        private async Task<User> CurrentDBUserAsync()
        {
            if (_httpContextAccessor.HttpContext.User == null)
                return null;

            var userIdString = _httpContextAccessor.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;

            if (!int.TryParse(userIdString, out int userId))
                return null;

            var user = await _myheartContext.Users.FindAsync(userId);

            if (user == null)
                return null;

            return user;
        }

        private async Task<User> DbUserByIdAsync(int userId)
        {
            if (userId < 0)
            {
                return null;
            }

            var user = await _myheartContext.Users.FindAsync(userId);

            return user;
        }

        public async Task<IEnumerable<UserDTO>> ListAsync(UType userType)
        {
            var users = await _myheartContext.Users
                .Where(x => x.Roles.Any(y => y.RoleType == userType) && x.DeleteDate == null)
                .ToListAsync();

            return _mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDTO> RegisterUserAsync(RegisterDTO registerViewModel)
        {
            var old = await _myheartContext.Users
                .Where(x => x.Email == registerViewModel.Email)
                .FirstOrDefaultAsync();

            if (old != null)
            {
                _myheartContext.Remove(old);
            }

            var newPassword = PasswordHasher.CreatePasswordHash(
                registerViewModel.Password,
                _securitySettings.Pepper
            );

            var user = new User()
            {
                Email = registerViewModel.Email,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                PasswordHash = newPassword,
                UserType = UType.Patient, //Seeded data,
                Guid = Guid.NewGuid(),
                Created = DateTime.Now
            };

            var stripeCustomer = await _paymentGatewayProvider.CreateCustomer(user);

            user.StripeId = stripeCustomer?.Id;

            _myheartContext.Add(user);

            var userRole = new Role() { UserId = user.Id, RoleType = UType.Patient, };
            _myheartContext.Add(userRole);

            await _myheartContext.SaveChangesAsync();

            var link = CreateActivationLink(user.Guid);
            var template = await _myheartContext.EmailTemplate
                .Where(x => x.Id == 1)
                .FirstOrDefaultAsync();

            string body = template.Body.Replace("www.google.com", link);

            await _emailService.SendMail(user.Email, "Aktivace hesla", body);

            return _mapper.Map<User, UserDTO>(user);
        }

        public async Task<UserDTO> ActivateUser(Guid token)
        {
            DbFunctions dfunc = null;
            var user = await _myheartContext.Users.FirstOrDefaultAsync(
                u =>
                    dfunc.DateDiffMinute(u.Created, DateTime.Now) <= 30
                    && u.Guid == token
                    && u.EmailComfirmed == false
            );

            if (user == null)
            {
                return null;
            }

            user.EmailComfirmed = true;

            await _myheartContext.SaveChangesAsync();

            return _mapper.Map<UserDTO>(user);
        }

        private string CreateActivationLink(Guid guid)
        {
            var url = _configuration.GetValue<string>("PasswordActivationUrl");

            url = $"{url}?token={guid}";

            return url;
        }

        private async Task<PasswordResetTicket> ValidateConfirmString(string token)
        {
            //DbFunctions dFunctions = null;

            var dbTickets = await _myheartContext.PasswordResetTickets
                .Where(t => DateTime.Now <= t.ExpireDate && t.TokenUsed == false)
                .ToListAsync();

            var dbTicket = dbTickets.SingleOrDefault(t => token == t.Token);

            return dbTicket;
        }

        public async Task<UserDTO> RecoverPassword(PasswordRecoverDTO recoverDTO)
        {
            var dbTicket = await ValidateConfirmString(recoverDTO.ConfirmString);

            if (dbTicket == null)
            {
                return null;
            }

            var dbUser = await _myheartContext.Users
                .OrderByDescending(u => u.Created)
                .FirstOrDefaultAsync(u => u.Email == dbTicket.Email);

            if (dbUser == null)
            {
                return null;
            }

            //we can confirm email sice it was successfully used for recovery
            if (!dbUser.EmailComfirmed)
            {
                dbUser.EmailComfirmed = true;
            }

            var newPassword = PasswordHasher.CreatePasswordHash(
                recoverDTO.Password,
                _securitySettings.Pepper
            );

            dbUser.PasswordHash = newPassword;

            dbTicket.TokenUsed = true;

            await _myheartContext.SaveChangesAsync();

            return _mapper.Map<UserDTO>(dbUser);
        }

        public async Task<bool> InvalidatePassword(int userId)
        {
            var user = await DbUserByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            user.PasswordHash = "";

            await _myheartContext.SaveChangesAsync();

            await SendRecoverPasswordLink(user.Email);

            return true;
        }

        public async Task<bool> SendRecoverPasswordLink(string email)
        {
            var dbUser = await _myheartContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (dbUser == null)
                return false;

            var body =
                $"Dobrý den,<br/><br/>"
                + $"Požádal/a jste o obnovení hesla v aplikaci MyHeart.<br/><br/>"
                + $"Heslo obnovíte kliknutím na odkaz:<br/><a href='{await MakePasswordRecoveryLink(dbUser, 15)}'>Obnovit heslo</a><br/><br/>"
                + $"Pokud jste o obnovení hesla nežádali tak prosím tento email ignorujte.<br/><br/>"
                + $"S pozdravem,<br/>"
                + $"Tým MyHeart";

            await _emailService.SendMail(dbUser.Email, "Obnova hesla", body);

            return true;
        }

        public async Task<string> MakePasswordRecoveryLink(User user, int recoveryTimeoutMinutes)
        {
            string token = AuthenticationService.CryptoSafeGuid().ToString("N");
            string confirmUrl = _configuration.GetValue<string>("PasswordRecoveryUrl");

            var resetTicket = new PasswordResetTicket
            {
                Email = user.Email,
                ExpireDate = DateTime.Now.AddMinutes(recoveryTimeoutMinutes),
                Token = token,
                TokenUsed = false
            };

            _myheartContext.PasswordResetTickets.Add(resetTicket);
            await _myheartContext.SaveChangesAsync();

            return $"{confirmUrl}?token={Uri.EscapeDataString(token)}";
        }

        public async Task<IEnumerable<QuestionListDTO>> GetUserQuestions(int userId)
        {
            if (userId == -1)
            {
                var questionList = await _myheartContext.Questions
                    .Include(q => q.Comments)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<Question>, IEnumerable<QuestionListDTO>>(
                    questionList
                );
            }
            else
            {
                var questionList = await _myheartContext.Questions
                    .Include(q => q.Comments)
                    .Where(q => q.UserId == userId)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<Question>, IEnumerable<QuestionListDTO>>(
                    questionList
                );
            }
        }

        public async Task<bool> AddNumberToUser(int id, string number)
        {
            var user = await _myheartContext.Users
                .Where(x => x.Id == id)
                .Include(x => x.UserDetail)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return false;
            }

            if (user.MFAConfirmed)
            {
                return false;
            }

            if (user.UserDetail == null)
            {
                user.UserDetail = new UserDetail()
                {
                    UserId = user.Id,
                    PIN = "",
                    SubscriptionId = 1
                };
            }

            user.UserDetail.Phone = number;
            await _myheartContext.SaveChangesAsync();

            return true;
        }

        public async Task<UserDetailDTO> GetUserDetail(int userId)
        {
            var personalDiseases = _myheartContext.UserPersonalDisease
                .Where(d => d.UserId == userId)
                .Select(d => new UserPersonalDiseaseDTO() { Name = d.Name, UserId = d.UserId })
                .ToList();
            var personalNonpharmaceutical = _myheartContext.UserPersonalNonpharmaceutical
                .Where(d => d.UserId == userId)
                .Select(d => new UserPersonalNonpharmaticDTO() { Name = d.Name, UserId = d.UserId })
                .ToList();
            var userPharmacy = _myheartContext.UserPharmacy
                .Where(up => up.UserId == userId)
                .Select(
                    up =>
                        new UserPharmacyDTO()
                        {
                            Name = up.Name,
                            UserId = up.UserId,
                            Dosing = up.Dosing
                        }
                )
                .ToList();

            var activeSubscription = await _paymentGatewayProvider.GetActiveSubscription(userId);
            var cancelAtPeriodEnd = activeSubscription?.CancelAtPeriodEnd;

            var user = await _myheartContext.Users
                .Include(u => u.UserDetail)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var userDetail = new UserDetailDTO
            {
                Id = user.Id,
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                StripeId = user.StripeId,
                SubscriptionCancelAtPeriodEnd = cancelAtPeriodEnd,
                Personal_Diseases = personalDiseases,
                Personal_NonpharmaticId = personalNonpharmaceutical,
                Pharmacies = userPharmacy,
            };

            if (user.UserDetail != null)
            {
                var detail = user.UserDetail;

                userDetail.DoctorId = detail.DoctorId;
                userDetail.SubscriptionId = detail.SubscriptionId;
                if (detail.Subscription != null)
                {
                    userDetail.Subscription = _mapper.Map<SubscriptionDTO>(detail.Subscription);
                }
                userDetail.PIN = detail.PIN;
                userDetail.Phone = detail.Phone;
                userDetail.Street = detail.Street;
                userDetail.StreetNumber = detail.StreetNumber;
                userDetail.City = detail.City;
                userDetail.Zip = detail.Zip;
                userDetail.InsuranceCompanyCode = detail.InsuranceCompanyCode;
                userDetail.InsuranceNumber = detail.InsuranceNumber;
                userDetail.SubscriptionValidTo = detail.SubscriptionValidTo;
                userDetail.IsAbusus_Smoker = true;
            }

            return userDetail;
        }

        public async Task<IEnumerable<QuestionCommentDTO>> GetQuestionComments(int questionId)
        {
            var questionComments = await _myheartContext.QuestionComments
                .Select(
                    q =>
                        new QuestionCommentDTO
                        {
                            QuestionId = q.QuestionId,
                            CreationDate = q.CreationDate,
                            Id = q.Id,
                            Text = q.Text
                        }
                )
                .Where(q => q.QuestionId == questionId)
                .ToListAsync();

            return questionComments;
        }

        public async Task<UserDTO> DeleteUser(int userId)
        {
            var user = await _myheartContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            user.DeleteDate = DateTime.Now;
            user.FirstName = "";
            user.LastName = "";
            //because emails have to be unique we need to have some unique value here and userID should be unique
            user.Email = $"DELETEDUID{user.Id}";
            //we shouldn't keep password hashes probably?
            user.PasswordHash = "";
            await _myheartContext.SaveChangesAsync();

            await _paymentGatewayProvider.CancelSubscription(user.StripeId);

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDetailDTO> UpdateUser(UserDetailDTO user, string userName)
        {
            var dbuser = await _myheartContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            var dbuserDetail = await _myheartContext.UserDetail.FirstOrDefaultAsync(
                u => u.UserId == user.Id
            );
            var dbuserAnamnesis = await _myheartContext.UserAnamnesis.FirstOrDefaultAsync(
                u => u.UserId == user.Id
            );
            var dbuserPersonalDiseases = await _myheartContext.UserPersonalDisease
                .Where(d => d.UserId == user.UserId)
                .ToListAsync();
            var dbuserPersonalNonpharmaceutical =
                await _myheartContext.UserPersonalNonpharmaceutical
                    .Where(d => d.UserId == user.UserId)
                    .ToListAsync();
            var dbuserPharmacies = await _myheartContext.UserPharmacy
                .Where(p => p.UserId == user.UserId)
                .ToListAsync();

            if (dbuser == null)
            {
                return null;
            }
            if (dbuserDetail != null && dbuserAnamnesis != null)
            {
                dbuser.FirstName = user.FirstName;
                dbuser.LastName = user.LastName;
                dbuser.LastUpdateUser = userName;

                dbuserAnamnesis.IsAbususAnamnesis = user.IsAbususAnamnesis;
                dbuserAnamnesis.IsAllergyAnamnesis = user.IsAllergyAnamnesis;
                dbuserAnamnesis.IsFamilyAnamnesis = user.IsFamilyAnamnesis;
                dbuserAnamnesis.IsPersonalAnamnesis = user.IsPersonalAnamnesis;
                dbuserAnamnesis.IsPharmacyAnamnesis = user.IsPharmacyAnamnesis;
                dbuserAnamnesis.IsSocialAnamnesis = user.IsSocialAnamnesis;
                dbuserDetail.SubscriptionId = user.SubscriptionId;
                dbuserDetail.PIN = user.PIN;
                dbuserDetail.DoctorId = user.DoctorId;
                dbuserAnamnesis.IsAbusus_Alcohol = user.IsAbusus_Alcohol;
                dbuserAnamnesis.IsAbusus_Exsmoker = user.IsAbusus_Exsmoker;
                dbuserAnamnesis.IsAbusus_Smoker = user.IsAbusus_Smoker;
                dbuserAnamnesis.IsAllergy_Name = user.IsAllergy_Name;
                dbuserAnamnesis.IsFamily_AtrialFibrillation = user.IsFamily_AtrialFibrillation;
                dbuserAnamnesis.IsFamily_ICHS = user.IsFamily_ICHS;
                dbuserAnamnesis.IsFamily_Pacemaker = user.IsFamily_Pacemaker;
                dbuserAnamnesis.IsFamily_SuddenDeath = user.IsFamily_SuddenDeath;
                dbuserAnamnesis.IsFamily_ValveDefect = user.IsFamily_ValveDefect;
                dbuserAnamnesis.IsFamily_HeartAttack = user.IsFamily_HeartAttack;
                dbuserAnamnesis.IsSocial_DisabilityPension = user.IsSocial_DisabilityPension;
                dbuserAnamnesis.IsSocial_LivingWithPartner = user.IsSocial_LivingWithPartner;
                dbuserAnamnesis.IsSocial_PartialDisabilityPension =
                    user.IsSocial_PartialDisabilityPension;
                dbuserAnamnesis.IsSocial_Pension = user.IsSocial_Pension;
                dbuserAnamnesis.IsSocial_Working = user.IsSocial_Working;
                dbuserAnamnesis.LastUpdateUser = userName;

                dbuserDetail.Phone = user.Phone;
                dbuserDetail.Street = user.Street;
                dbuserDetail.StreetNumber = user.StreetNumber;
                dbuserDetail.City = user.City;
                dbuserDetail.Zip = user.Zip;
                dbuserDetail.LastUpdateUser = userName;
                dbuserDetail.InsuranceCompanyCode = user.InsuranceCompanyCode;
                dbuserDetail.InsuranceNumber = user.InsuranceNumber;
            }
            else
            {
                if (dbuserDetail == null)
                {
                    var userDetail = new UserDetail()
                    {
                        DoctorId = user.DoctorId,
                        UserId = user.UserId,
                        SubscriptionId = user.SubscriptionId == 0 ? 1 : user.SubscriptionId,
                        PIN = user.PIN == null ? "" : user.PIN,
                        Phone = user.Phone,
                        Street = user.Street,
                        StreetNumber = user.StreetNumber,
                        City = user.City,
                        Zip = user.Zip,
                        LastUpdateUser = userName,
                        InsuranceCompanyCode = user.InsuranceCompanyCode,
                        InsuranceNumber = user.InsuranceNumber
                    };
                    _myheartContext.Add(userDetail);
                }

                if (dbuserAnamnesis == null)
                {
                    var userAnamnesis = new UserAnamnesis()
                    {
                        UserId = user.UserId,
                        IsAbususAnamnesis = user.IsAbususAnamnesis,
                        IsAllergyAnamnesis = user.IsAllergyAnamnesis,
                        IsFamilyAnamnesis = user.IsFamilyAnamnesis,
                        IsPersonalAnamnesis = user.IsPersonalAnamnesis,
                        IsPharmacyAnamnesis = user.IsPharmacyAnamnesis,
                        IsSocialAnamnesis = user.IsSocialAnamnesis,
                        IsAbusus_Alcohol = user.IsAbusus_Alcohol,
                        IsAbusus_Exsmoker = user.IsAbusus_Exsmoker,
                        IsAbusus_Smoker = user.IsAbusus_Smoker,
                        IsAllergy_Name = user.IsAllergy_Name,
                        IsFamily_AtrialFibrillation = user.IsFamily_AtrialFibrillation,
                        IsFamily_ICHS = user.IsFamily_ICHS,
                        IsFamily_Pacemaker = user.IsFamily_Pacemaker,
                        IsFamily_SuddenDeath = user.IsFamily_SuddenDeath,
                        IsFamily_ValveDefect = user.IsFamily_ValveDefect,
                        IsFamily_HeartAttack = user.IsFamily_HeartAttack,
                        IsSocial_DisabilityPension = user.IsSocial_DisabilityPension,
                        IsSocial_LivingWithPartner = user.IsSocial_LivingWithPartner,
                        IsSocial_PartialDisabilityPension = user.IsSocial_PartialDisabilityPension,
                        IsSocial_Pension = user.IsSocial_Pension,
                        IsSocial_Working = user.IsSocial_Working,
                        LastUpdateUser = userName
                    };
                    _myheartContext.Add(userAnamnesis);
                }
            }

            {
                // update Personal Diseases
                foreach (var personalDisease in user.Personal_Diseases)
                {
                    var modelToRemove = dbuserPersonalDiseases.FirstOrDefault(
                        savedModel => savedModel.Name == personalDisease.Name
                    );
                    if (modelToRemove != null)
                    {
                        dbuserPersonalDiseases.Remove(modelToRemove);
                        personalDisease.Name = null;
                    }
                }
                // remove the ones with no need to update
                for (int i = 0; i < user.Personal_Diseases.Count; i++)
                {
                    if (user.Personal_Diseases[i].Name == null)
                    {
                        user.Personal_Diseases.RemoveAt(i);
                        i--;
                    }
                }
                //remove saved in DB, but not received from client
                foreach (var oldDisease in dbuserPersonalDiseases)
                {
                    _myheartContext.Remove(oldDisease);
                }
                // add new
                user.Personal_Diseases.ForEach(
                    disease =>
                        _myheartContext.Add(
                            new UserPersonalDisease()
                            {
                                Name = disease.Name,
                                UserId = disease.UserId
                            }
                        )
                );
            }
            {
                // update Personal Nonpharmaceutical
                foreach (var personalNonpharmaceutical in user.Personal_NonpharmaticId)
                {
                    // no need to update
                    var modelToRemove = dbuserPersonalNonpharmaceutical.FirstOrDefault(
                        savedModel => savedModel.Name == personalNonpharmaceutical.Name
                    );
                    if (modelToRemove != null)
                    {
                        dbuserPersonalNonpharmaceutical.Remove(modelToRemove);
                        personalNonpharmaceutical.Name = null;
                    }
                }
                // remove the ones with no need to update
                for (int i = 0; i < user.Personal_NonpharmaticId.Count; i++)
                {
                    if (user.Personal_NonpharmaticId[i].Name == null)
                    {
                        user.Personal_NonpharmaticId.RemoveAt(i);
                        i--;
                    }
                }
                //remove saved in DB, but not received from client
                foreach (var oldNonpharmaceutical in dbuserPersonalNonpharmaceutical)
                {
                    _myheartContext.Remove(oldNonpharmaceutical);
                }
                // add new
                user.Personal_NonpharmaticId.ForEach(
                    newNonpharma =>
                        _myheartContext.Add(
                            new UserPersonalNonpharmaceutical()
                            {
                                Name = newNonpharma.Name,
                                UserId = newNonpharma.UserId
                            }
                        )
                );
            }
            {
                // update Pharmacies
                foreach (var pharma in user.Pharmacies)
                {
                    var modelToRemove = dbuserPharmacies.FirstOrDefault(
                        savedModel => savedModel.Name == pharma.Name
                    );
                    if (modelToRemove != null)
                    {
                        dbuserPharmacies.Remove(modelToRemove);
                        pharma.Name = null;
                    }
                }
                // remove the ones with no need to update
                for (int i = 0; i < user.Pharmacies.Count; i++)
                {
                    if (user.Pharmacies[i].Name == null)
                    {
                        user.Pharmacies.RemoveAt(i);
                        i--;
                    }
                }
                //remove saved in DB, but not received from client
                foreach (var oldPharma in dbuserPharmacies)
                {
                    _myheartContext.Remove(oldPharma);
                }
                // add new
                user.Pharmacies.ForEach(
                    newPharma =>
                        _myheartContext.Add(
                            new UserPharmacy()
                            {
                                Name = newPharma.Name,
                                UserId = newPharma.UserId,
                                Dosing = newPharma.Dosing
                            }
                        )
                );
            }

            await _myheartContext.SaveChangesAsync();

            return _mapper.Map<UserDetailDTO>(user);
        }

        public async Task<UserNotificationSettingsDTO> GetUserNotificationSettings()
        {
            var user = await CurrentDBUserAsync();

            if (user == null)
            {
                return null;
            }

            return new UserNotificationSettingsDTO
            {
                UserID = user.Id,
                ReplyEmailNotification = user.ReplyEmailNotification,
                TherapyNewsEmailNotification = user.TherapyNewsEmailNotification,
                SubscriptionPreferences = user.SubscriptionPreferences
            };
        }

        public async Task<UserNotificationSettingsDTO> GetUserNotificationSettings(int userId)
        {
            var user = await DbUserByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            return new UserNotificationSettingsDTO
            {
                UserID = user.Id,
                ReplyEmailNotification = user.ReplyEmailNotification,
                TherapyNewsEmailNotification = user.TherapyNewsEmailNotification,
                SubscriptionPreferences = user.SubscriptionPreferences
            };
        }

        public async Task<UserNotificationSettingsDTO> UpdateUserNotificationSettings(
            UserNotificationSettingsDTO settings
        )
        {
            var user = await DbUserByIdAsync(settings.UserID);

            user.TherapyNewsEmailNotification = settings.TherapyNewsEmailNotification;
            user.SubscriptionPreferences = settings.SubscriptionPreferences;
            user.ReplyEmailNotification = settings.ReplyEmailNotification;

            _myheartContext.Update(user);
            await _myheartContext.SaveChangesAsync();

            return new UserNotificationSettingsDTO
            {
                UserID = user.Id,
                ReplyEmailNotification = user.ReplyEmailNotification,
                TherapyNewsEmailNotification = user.TherapyNewsEmailNotification,
                SubscriptionPreferences = user.SubscriptionPreferences
            };
        }

        public async Task<int> GetUserIdByEmail(string email)
        {
            var user = await _myheartContext.Users
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return -1;
            }

            return user.Id;
        }

        public async Task<UserFmcTokenDTO> AddFmcToken(UserFmcTokenDTO tokenDto)
        {
            var userToken = _mapper.Map<UserFmcToken>(tokenDto);

            var fmcToken = await _myheartContext.UserFmcTokens.FirstOrDefaultAsync(
                x => x.UserId == tokenDto.UserId
            );
            if (fmcToken == null)
            {
                _myheartContext.Add(userToken);
            }
            else
            {
                fmcToken.Token = tokenDto.Token;
            }

            await _myheartContext.SaveChangesAsync();

            return _mapper.Map<UserFmcTokenDTO>(userToken);
        }

        public async Task<IEnumerable<UserFullDTO>> GetForPatientsExport()
        {
            var users = await _myheartContext.Users
                .Include(u => u.UserAnamnesis)
                .Include(u => u.UserNonpharmacy)
                .Include(u => u.Questions)
                .Include(u => u.UserReports)
                .Include(u => u.UserDetail)
                .Where(u => u.Roles.Any(x => x.RoleType == UType.Patient))
                .Where(u => u.DeleteDate == null)
                .ToListAsync();

            return users.Select(_mapper.Map<UserFullDTO>);
        }

        public async Task<IEnumerable<UserFullDTO>> GetDoctorsForExport()
        {
            var users = await _myheartContext.Users
                .Include(u => u.Questions)
                .Where(u => u.Roles.Any(x => x.RoleType == UType.Doctor))
                .Where(u => u.DeleteDate == null)
                .ToListAsync();

            return users.Select(_mapper.Map<UserFullDTO>);
        }

        #region Validation

        public async Task<Dictionary<string, string>> Validate(RegisterDTO registerViewModel)
        {
            var errorList = new Dictionary<string, string>();

            if (!EmailValidator.Validate(registerViewModel.Email))
                errorList.Add("Email", "Provided email is not valid");

            if (
                await _myheartContext.Users.AnyAsync(
                    u =>
                        u.Email == registerViewModel.Email
                        && u.EmailComfirmed == true
                        && u.MFAConfirmed
                )
            )
                errorList.Add("Email", "Email already exists");

            return errorList;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>IEnumerable<UserReportDTO> with empty files for optimization. Use method GetUserReportFiles to get files.</returns>
        public async Task<IEnumerable<UserReportDTO>> GetUserReports(int userId)
        {
            var values = GetEnumList<ReportTypeDTO>();

            var reports = await _myheartContext.UserReport
                .Where(ur => ur.UserId == userId)
                .Select(
                    ur =>
                        new UserReport()
                        {
                            Id = ur.Id,
                            Title = ur.Title,
                            UserId = ur.UserId,
                            LastUpdate = ur.LastUpdate,
                            CreationDate = ur.CreationDate,
                            Description = ur.Description,
                            ReportType = ur.ReportType,
                            UserReportFiles = ur.UserReportFiles
                                .Select(f => new UserReportFile { Extension = f.Extension, })
                                .ToList()
                        }
                )
                .ToListAsync();

            return reports.Select(_mapper.Map<UserReportDTO>);
        }

        public async Task<UserReportDTO> AddUserReport(
            int userId,
            UserReportDTO report,
            string userName
        )
        {
            if (
                report.UserId == userId /*&& !_myheartContext.UserReport.Any(ur => ur.LastUpdate == report.LastUpdate)*/
            )
            {
                var user = _myheartContext.Users.FirstOrDefault(u => u.Id == userId);

                var ur = new UserReport()
                {
                    UserId = report.UserId,
                    Title = report.Title,
                    LastUpdate = report.LastUpdate,
                    CreationDate = report.CreationDate,
                    User = user,
                    ReportType = GetEnumList<ReportType>()[(int)report.ReportType],
                    Description = report.Description,
                    LastUpdateUser = userName
                };
                _myheartContext.UserReport.Add(ur);

                await _myheartContext.SaveChangesAsync();

                report.Files.ForEach(
                    f =>
                        _myheartContext.UserReportFile.Add(
                            new UserReportFile()
                            {
                                Name = f.Name,
                                Extension = f.Extension,
                                Content = f.Content,
                                UserReport = ur,
                                UserReportId = ur.Id,
                                LastUpdateUser = userName
                            }
                        )
                );

                await _myheartContext.SaveChangesAsync();

                return _mapper.Map<UserReportDTO>(report);
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<UserReportFileDTO>> GetUserReportFiles(
            int userId,
            int reportId
        )
        {
            //var userReport = _myheartContext.UserReport
            //        .FirstOrDefault(ur => ur.UserId == userId && ur.Id == reportId);
            var files = await _myheartContext.UserReportFile
                .Where(urf => urf.UserReport.UserId == userId && urf.UserReportId == reportId)
                .Select(
                    urf =>
                        new UserReportFileDTO()
                        {
                            Name = urf.Name,
                            Extension = urf.Extension,
                            Content = urf.Content
                        }
                )
                .ToArrayAsync();
            return files;
        }

        public async Task<bool> DeleteUserReport(int userId, int reportId)
        {
            var report = GetReport(userId, reportId);

            await _myheartContext.UserReportFile
                .Where(urf => urf.UserReportId == reportId)
                .ForEachAsync(urf => _myheartContext.UserReportFile.Remove(urf));
            _myheartContext.UserReport.Remove(report);

            await _myheartContext.SaveChangesAsync();

            return true;
        }

        private static TEnum[] GetEnumList<TEnum>()
            where TEnum : Enum => (TEnum[])Enum.GetValues(typeof(TEnum));

        public async Task<bool> UpdateReport(
            int userId,
            int reportId,
            UserReportUpdateDTO reportUpdateDTO
        )
        {
            if (reportUpdateDTO.Title == null || reportUpdateDTO.Description == null)
            {
                return false;
            }

            var report = GetReport(userId, reportId);

            report.Title = reportUpdateDTO.Title;
            report.Description = reportUpdateDTO.Description;
            report.CreationDate = reportUpdateDTO.CreationDate;
            report.ReportType = GetEnumList<ReportType>()[(int)reportUpdateDTO.ReportType];
            report.LastUpdate = DateTime.UtcNow;

            await _myheartContext.SaveChangesAsync();

            return true;
        }

        private UserReport GetReport(int userId, int reportId) =>
            _myheartContext.UserReport.First(ur => ur.UserId == userId && ur.Id == reportId);

        #endregion Validation
    }
}
