using MyHeart.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User
{
    public class UserPersonalNonpharmaticDTO : BaseEntityDTO
    {
        public string Name { get; set; }
        public int UserId { get; set; }
    }
}
