using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.User
{
    public class UserAnamnesisDTO : BaseEntityDTO
    {
        public int UserId { get; set; }

        //  Family
        public bool IsFamilyAnamnesis { get; set; }
        public bool IsFamily_ICHS { get; set; }
        public bool IsFamily_ValveDefect { get; set; }
        public bool IsFamily_AtrialFibrillation { get; set; }
        public bool IsFamily_SuddenDeath { get; set; }
        public bool IsFamily_Pacemaker { get; set; }
        public bool IsFamily_HeartAttack { get; set; }
        //  Personal
        public bool IsPersonalAnamnesis { get; set; }
        public int IsPersonal_Type { get; set; }
        public DateTime IsPersonal_Date { get; set; }
        public string IsPersonal_Value { get; set; }
        public PersonalAnamnesisCreatorType IsPersonal_CreatorType { get; set; }

        //  Pharmacy
        public bool IsPharmacyAnamnesis { get; set; }
        public int IsPharmacy_PharmacyId { get; set; }
        public string IsPharmacy_Name { get; set; }
        public string IsPharmacy_Dose { get; set; }
        public string IsPharmacy_MorningDose { get; set; }
        public string IsPharmacy_AfternoonDose { get; set; }
        public string IsPharmacy_EveningDose { get; set; }
        public string IsPharmacy_Note { get; set; }

        //  Allergy
        public bool IsAllergyAnamnesis { get; set; }
        public string IsAllergy_Name { get; set; }

        //  Abusus
        public bool IsAbususAnamnesis { get; set; }
        public bool IsAbusus_Alcohol { get; set; }
        public bool IsAbusus_Exsmoker { get; set; }
        public bool IsAbusus_Smoker { get; set; }

        //  Social
        public bool IsSocialAnamnesis { get; set; }

        public bool IsSocial_LivingWithPartner { get; set; }
        public bool IsSocial_Working { get; set; }
        public bool IsSocial_Pension { get; set; }
        public bool IsSocial_PartialDisabilityPension { get; set; }
        public bool IsSocial_DisabilityPension { get; set; }

        //General 
        public bool IsGeneralAnamnesis { get; set; }
        public PhysicalActivityFrequencyType IsGeneral_PhysicalActivityFrequencyType { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public enum PhysicalActivityFrequencyType
    {
        None,
        Once,
        Twice,
        FourTimes,
        SixTimes
    }
    public enum PersonalAnamnesisCreatorType
    {
        User,
        HealthKit,
        GoogleFit
    }
}