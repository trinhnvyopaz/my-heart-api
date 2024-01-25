using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User
{
    public class UserPharmacyDTO : BaseEntityDTO
    {
        public string Name { get; set; }
        public int UserId { get; set; }
        public string Dosing { get; set; }
    }
}
