using AutoMapper;
using MyHeart.Core.Helpers;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.Services.Interfaces;
using MyHeart.DTO.User;
using MyHeart.DTO.Doctor;
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
using System.Text;
using MyHeart.DTO;
using Microsoft.Extensions.Options;

namespace MyHeart.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MyHeartContext _myheartContext;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly SecuritySettings _securitySettings;
        private readonly IEmailService _emailService;
        private static Random random = new Random();

        public DoctorService(IHttpContextAccessor httpContextAccessor, MyHeartContext myheartContext, IMapper mapper, IConfiguration configuration, IEmailService emailService, IUserService userService, IOptions<SecuritySettings> options)
        {
            _httpContextAccessor = httpContextAccessor;
            _myheartContext = myheartContext;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
            _userService = userService;
            _securitySettings = options.Value;
        }

        public async Task<UserDTO> RegisterUserAsync(RegisterDoctorDTO registerViewModel)
        {
            string password = RandomString(16);

            var newPassword = PasswordHasher.CreatePasswordHash(password, _securitySettings.Pepper);

            var user = new User()
            {
                Email = registerViewModel.Email,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                PasswordHash = newPassword,
                UserType = UType.Doctor, //Doctor,
                Guid = Guid.NewGuid(),
                Created = DateTime.Now
            };

            _myheartContext.Add(user);

            await _myheartContext.SaveChangesAsync();


            var body = $"Dobrý den, <br/><br/>Dostal/a jste pozvánku do aplikace Moje srdce. " +
                $"<br/><br/>Registraci dokončíte kliknutím na odkaz<br/>" +
                $"{await _userService.MakePasswordRecoveryLink(user, 240)}<br/><br/>" +
                $"Pokud email není určen Vám tak ho prosím ignorujte.<br/><br/>" +
                $"S pozdravem,<br/>Tým Moje srdce";

            await _emailService.SendMail(user.Email, "Aktivace hesla", body);

            return _mapper.Map<User, UserDTO>(user);
        }
        private string CreateActivationLink(Guid guid)
        {
            var url = _configuration.GetValue<string>("PasswordActivationUrl");

            url = $"{url}?token={guid}";

            return url;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public async Task<DoctorDetailDTO> UpdateDoctor(DoctorDetailDTO doctor, string userName)
        {
            var dbDoctor = await _myheartContext.Users.FirstOrDefaultAsync(u => u.Id == doctor.UserId);
            var dbDoctorDetail = await _myheartContext.DoctorDetails.FirstOrDefaultAsync(u => u.UserId == doctor.Id);

            if (dbDoctor == null)// || dbDoctorDetail == null)
            {
                return null;
            }

            dbDoctor.FirstName = doctor.FirstName;
            dbDoctor.LastName = doctor.LastName;
            dbDoctor.LastUpdateUser = userName;

            await _myheartContext.SaveChangesAsync();

            if(dbDoctorDetail == null) {
                dbDoctorDetail = new DoctorDetail() {
                    UserId = doctor.UserId,
                    Phone = doctor.Phone,
                    Description = doctor.Description,
                    WorkPlace = doctor.WorkPlace,
                    LastUpdateUser = userName
                };

                _myheartContext.DoctorDetails.Add(dbDoctorDetail);
            } else {
                dbDoctorDetail.Phone = doctor.Phone;
                dbDoctorDetail.Description = doctor.Description;
                dbDoctorDetail.WorkPlace = doctor.WorkPlace;
                dbDoctorDetail.LastUpdateUser = userName;
            }

            await _myheartContext.SaveChangesAsync();
            return _mapper.Map<DoctorDetailDTO>(doctor);
        }

        public async Task<DoctorDetailDTO> GetDoctorDetail(int doctorId)
        {
            var doctorDetail = await _myheartContext.DoctorDetails.FirstOrDefaultAsync(x => x.Id == doctorId);

            if (doctorDetail == null) return null;

            var userDetail = await _myheartContext.Users.FirstOrDefaultAsync(q => q.Id == doctorDetail.UserId);

            return new DoctorDetailDTO() { 
                Id = doctorDetail.Id,
                UserId = userDetail.Id,
                FirstName = userDetail.FirstName,
                LastName = userDetail.LastName,
                Email = userDetail.Email,
                Phone = doctorDetail.Phone,
                Description = doctorDetail.Description,
                WorkPlace = doctorDetail.WorkPlace,
                LastUpdateDate = doctorDetail.LastUpdateDate,
                LastUpdateUser = doctorDetail.LastUpdateUser
            };
        }

        public async Task<DoctorDetailDTO> GetDoctorDetailByUserID(int userID) {
            var userDetail = await _myheartContext.Users.FirstOrDefaultAsync(q => q.Id == userID);

            if (userDetail == null) return null;

            var doctorDetail = await _myheartContext.DoctorDetails.FirstOrDefaultAsync(x => x.UserId == userID);

            var doctor = new DoctorDetailDTO() {
                UserId = userDetail.Id,
                FirstName = userDetail.FirstName,
                LastName = userDetail.LastName,
                Email = userDetail.Email
            };

            if (doctorDetail != null) {
                doctor.Id = doctorDetail.Id;
                doctor.Phone = doctorDetail.Phone;
                doctor.Description = doctorDetail.Description;
                doctor.WorkPlace = doctorDetail.WorkPlace;
                doctor.LastUpdateDate = doctorDetail.LastUpdateDate;
                doctor.LastUpdateUser = doctorDetail.LastUpdateUser;
            }

            return doctor;
        }

        #region Validation

        public async Task<Dictionary<string, string>> Validate(RegisterDoctorDTO registerViewModel)
        {
            var errorList = new Dictionary<string, string>();

            if (!EmailValidator.Validate(registerViewModel.Email))
                errorList.Add("Email", "Email není validní");

            if (await _myheartContext.Users.AnyAsync(u => u.Email == registerViewModel.Email))
                errorList.Add("Email", "Email už existuje");

            return errorList;
        }

        #endregion Validation
    }
}