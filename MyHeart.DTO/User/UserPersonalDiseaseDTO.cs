using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User
{
    public class UserPersonalDiseaseDTO : BaseEntityDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; }
    }
}
