using MyHeart.DTO.MedicalReport;
using MyHeart.DTO.Questions;
using System;

namespace MyHeart.DTO.User
{
    public class UserDTO : BaseEntityDTO
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UType UserType { get; set; }
        public string StripeId { get; set; }
        public virtual QuestionListDTO Question { get; set; }
        public int SubscriptionPreferences { get; set; }
        public string UserTypes { get; set; }
        public string FullName  => $"{FirstName} {LastName}";
    }

    public enum UType {
        SuperAdmin,
        Admin,
        Doctor,
        Patient,
        DataManager
    }
  
}