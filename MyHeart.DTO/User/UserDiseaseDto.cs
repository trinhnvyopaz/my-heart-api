using MyHeart.DTO.ProfessionalInformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User
{
    public class UserDiseaseDto: BaseEntityDTO
    {
        public int UserId { get; set; }
        public UserDTO User { get; set; }

        public int DiseaseId { get; set; }
        public DiseaseDTO Disease { get; set; }

        public DateTime StartDate { get; set; }
        public string StartDateString { get; set; }
        public string Note { get; set; }
    }
}
