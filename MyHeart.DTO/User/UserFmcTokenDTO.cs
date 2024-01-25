using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User
{
    public class UserFmcTokenDTO : BaseEntityDTO
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}
