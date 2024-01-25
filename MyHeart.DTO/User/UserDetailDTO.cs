using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User
{
    public class UserDetailDTO : BaseEntityDTO
    {
        public string PIN { get; set; }
        public int DoctorId { get; set; }
        public int SubscriptionId { get; set; }
        public SubscriptionDTO Subscription { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }

        public int InsuranceCompanyCode { get; set; }
        public int InsuranceNumber { get; set; }

        public bool IsFamilyAnamnesis { get; set; }
        public bool IsPersonalAnamnesis { get; set; }
        public bool IsPharmacyAnamnesis { get; set; }
        public bool IsAllergyAnamnesis { get; set; }
        public bool IsAbususAnamnesis { get; set; }
        public bool IsSocialAnamnesis { get; set; }

        public bool IsFamily_ICHS { get; set; }
        public bool IsFamily_ValveDefect { get; set; }
        public bool IsFamily_AtrialFibrillation { get; set; }
        public bool IsFamily_SuddenDeath { get; set; }
        public bool IsFamily_Pacemaker { get; set; }
        public bool IsFamily_HeartAttack { get; set; }
        public List<UserPersonalDiseaseDTO> Personal_Diseases { get; set; }
        public List<UserPersonalNonpharmaticDTO> Personal_NonpharmaticId { get; set; }
        // medications with dosing
        public List<UserPharmacyDTO> Pharmacies { get; set; }

        //public int IsPharmacy_PharmacyId { get; set; }
        //public int IsPharmacy_BoldStr { get; set; }
        //public string IsPharmacy_Dose { get; set; }

        public string IsAllergy_Name { get; set; }

        public bool IsAbusus_Alcohol { get; set; }
        public bool IsAbusus_Exsmoker { get; set; }
        public bool IsAbusus_Smoker { get; set; }

        public bool IsSocial_LivingWithPartner { get; set; }
        public bool IsSocial_Working { get; set; }
        public bool IsSocial_Pension { get; set; }
        public bool IsSocial_PartialDisabilityPension { get; set; }
        public bool IsSocial_DisabilityPension { get; set; }
        public string StripeId { get; set; }
        public DateTime? SubscriptionValidTo { get; set; }
        public bool? SubscriptionCancelAtPeriodEnd { get; set; }
    }
}