using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User
{
    public class UserFmcDTO : UserDTO
    {
        public List<UserFmcTokenDTO> UserFmcTokens { get; set; }
    }
}
