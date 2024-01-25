using AutoMapper;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.Services.Interfaces;
using MyHeart.DTO.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services
{
    public class UserTypeService : IUserTypeService
    {
        private MyHeartContext _context;
        private IMapper _mapper;

        public UserTypeService(MyHeartContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /*public async Task<List<UserTypeDTO>> GetListAsync()
        {
            var list = await _context.UserTypes.ToListAsync();

            return _mapper.Map<List<UserType>, List<UserTypeDTO>>(list);
        }*/
    }
}
