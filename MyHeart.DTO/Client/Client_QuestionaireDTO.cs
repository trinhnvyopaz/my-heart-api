using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO
{
    public class Client_QuestionaireDTO : BaseEntityDTO
    {
        public int UserId { get; set; }
        public UserDTO User { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<Client_QuestionAnswerDTO> QuestionAnswers { get; set; }
    }
}
