using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyHeart.Data.Models
{
    public class User : BaseModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string MFASecret { get; set; }
        public long MFATimeSlice { get; set; }
        public string MFARecovery { get; set; }
        public string SMSMFA { get; set; }
        /*public int UserTypeId { get; set; }
        [Required]
        public UserType UserType { get; set; }*/
        public UType UserType { get; set; }
        public bool EmailComfirmed { get; set; }
        public bool MFAConfirmed { get; set; }
        public Guid Guid { get; set; }
        public DateTime Created { get; set; }
        public List<Question> Questions { get; set; }
        public UserDetail UserDetail { get; set; }
        public List<UserPersonalDisease> PersonalDisease { get; set; }
        public List<UserPersonalNonpharmaceutical> PersonalNonpharmaceutical { get; set; }
        public List<UserReport> UserReports { get; set; }
        public List<UserAnamnesis> UserAnamnesis { get; set; }
        public DoctorDetail DoctorDetail { get; set; }
        public DateTime? DeleteDate { get; set; }

        public List<Client_Disease> DiagnosedDiseases { get; set; }
        public List<UserTrustedLogin> TrustedLogins { get; set; }

        public int SubscriptionPreferences { get; set; }
        public bool TherapyNewsEmailNotification { get; set; }
        public bool ReplyEmailNotification { get; set; }

        public List<UserNonpharmacy> UserNonpharmacy { get; set; }
        public List<UserFmcToken> UserFmcTokens { get; set; }
        public List<UserDisease> UserDiseases { get; set; }
        public string StripeId { get; set; }
        public ICollection<Role> Roles { get; set; }
    }

    public enum PersonalAnamnesisType
    {
        BloodPressure = 1,
        Cholesterol = 2,
        LabResult = 3,
        Height = 4,
        Weight = 5,
        HeartRate = 6,
        Blood = 7,
    }
}