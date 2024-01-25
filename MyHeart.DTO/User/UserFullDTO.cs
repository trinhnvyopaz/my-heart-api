using MyHeart.DTO;
using MyHeart.DTO.Questions;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User
{
    public class UserFullDTO : UserDTO 
    {
        public List<QuestionListDTO> Questions { get; set; }
        public List<UserReportDTO> UserReports { get; set; }
        public List<UserNonpharmacyDto> UserNonpharmacy { get; set; }
        public List<UserDiseaseDto> UserDiseases { get; set; }
        public bool EmailComfirmed { get; set; }
        public DateTime Created { get; set; }
        public UserDetailDTO UserDetail { get; set; }
    }
}
