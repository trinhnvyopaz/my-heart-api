using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.DTO;
using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services
{
    public class UserNonpharmacyService : IUserNonpharmacyService
    {

        private readonly MyHeartContext _myheartContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public UserNonpharmacyService(MyHeartContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
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

        private async Task<User> GetUserById(int id) {
            return await _myheartContext.Users.FindAsync(id);
        }

        public async Task<IEnumerable<UserNonpharmacyDto>> GetUserNonpharmacies(int id) {
            var user = await GetUserById(id);

            return await GetUserNonpharmacies(user);
        }



        public async Task<IEnumerable<UserNonpharmacyDto>> GetUserNonpharmacies()
        {
            var user = await GetCurrentUser();

            return await GetUserNonpharmacies(user);
        }

        private async Task<IEnumerable<UserNonpharmacyDto>> GetUserNonpharmacies(User user) {
            if (user == null)
                return null;

            var result = await _myheartContext.UserNonpharmacy
                .Include(x => x.NonpharmaticTherapy)
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserNonpharmacy>, IEnumerable<UserNonpharmacyDto>>(result);
        }

        private async Task<UserNonpharmacyDto> GetUserNonpharmacyById(int id)
        {
            var result = await _myheartContext.UserNonpharmacy
                .Include(x => x.NonpharmaticTherapy)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (result == null)
                return null;

            return _mapper.Map<UserNonpharmacy, UserNonpharmacyDto>(result);
        }

        public async Task<UserNonpharmacyDto> SaveUserNonpharmacy(UserNonpharmacyDto model, string userName)
        {
            if(model.Id > 0)
            {
                var dbModel = await _myheartContext.UserNonpharmacy
                    .FirstOrDefaultAsync(x => x.Id == model.Id);

                if (dbModel == null)
                    return null;

                dbModel.NonpharmaticTherapyId = model.NonpharmaticTherapyId;
                dbModel.FacilityName = model.FacilityName;
                dbModel.Note = model.Note;
                dbModel.LastUpdateUser = userName;
                dbModel.LastUpdateDate = DateTime.Now;

                _myheartContext.Entry(dbModel).State = EntityState.Modified;
                await _myheartContext.SaveChangesAsync();

                return await GetUserNonpharmacyById(dbModel.Id);
            }
            else
            {
                var dbModel = new UserNonpharmacy
                {
                    UserId = model.UserId,
                    NonpharmaticTherapyId = model.NonpharmaticTherapyId,
                    FacilityName = model.FacilityName,
                    Note = model.Note,
                    LastUpdateUser = userName
                };

                _myheartContext.UserNonpharmacy.Add(dbModel);
                await _myheartContext.SaveChangesAsync();

                return await GetUserNonpharmacyById(dbModel.Id);
            }
        }

        public async Task<bool> DeleteUserNonpharmacy(int id)
        {
            var dbModel = _myheartContext.UserNonpharmacy
                .FirstOrDefault(x => x.Id == id);

            if (dbModel == null)
                return false;

            _myheartContext.UserNonpharmacy
                .Remove(dbModel);
            await _myheartContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DoesNonPharmacyBelongToUser(int userId, int nonPharmaId) {
            var nonPharma = _myheartContext.UserNonpharmacy.Find(nonPharmaId);

            if(nonPharma == null) {
                return false;
            }

            return userId == nonPharma.UserId;
        } 

        public async Task<IEnumerable<NonpharmacyDTO>> GetNonpharmaticTherapiesByName(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
                return null;

            var result = await _myheartContext.NonpharmaticTherapy
                .Where(x => x.Name.ToLower().Contains(searchString.ToLower()))
                .ToListAsync();

            return _mapper.Map<IEnumerable<NonpharmaticTherapy>, IEnumerable<NonpharmacyDTO>>(result);
        }
    }
}
