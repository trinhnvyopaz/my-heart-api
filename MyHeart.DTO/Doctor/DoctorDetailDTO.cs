using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User
{
    public class DoctorDetailDTO : BaseEntityDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public string WorkPlace { get; set; }
    }
}