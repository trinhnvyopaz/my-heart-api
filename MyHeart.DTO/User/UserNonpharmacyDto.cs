using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO
{
    public class UserNonpharmacyDto : BaseEntityDTO
    {
        public int UserId { get; set; }
        public UserDTO User { get; set; }
        public int NonpharmaticTherapyId { get; set; }
        public NonpharmacyDTO NonpharmaticTherapy { get; set; }

        public string FacilityName { get; set; }
        public string Note { get; set; }
    }
}
