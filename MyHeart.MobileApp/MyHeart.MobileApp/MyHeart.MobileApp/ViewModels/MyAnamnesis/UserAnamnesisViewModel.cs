using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.ViewModels
{
    public class UserAnamnesisViewModel : BaseViewModel, ICloneable
    {
        private string isPharmacy_MorningDose;
        private string isPharmacy_AfternoonDose;
        private string isPharmacy_EveningDose;
        private string isPharmacy_Note;
        private bool isAbusus_Smoker;
        private bool isFamily_HeartAttack;
        private string isPersonal_Value;
        private DateTime isPersonal_Date;
        private PhysicalActivityFrequencyType isGeneral_PhysicalActivityFrequencyType;
        private bool showDosageDetail;
        private bool isFamilyAnamnesis;
        private bool isFamily_ICHS;
        private bool isFamily_ValveDefect;
        private bool isFamily_AtrialFibrillation;
        private bool isFamily_SuddenDeath;
        private bool isFamily_Pacemaker;
        private bool isAbususAnamnesis;
        private bool isAbusus_Alcohol;
        private bool isAbusus_Exsmoker;
        private bool isSocialAnamnesis;
        private bool isSocial_LivingWithPartner;
        private bool isSocial_Working;
        private bool isSocial_Pension;
        private bool isSocial_PartialDisabilityPension;
        private bool isSocial_DisabilityPension;

        public int Id { get; set; }
        public int IsPharmacy_PharmacyId { get; set; }
        public string IsPharmacy_Name { get; set; }
        public string IsPharmacy_Dose { get; set; }

        public string FormatedDoseValue
        {
            get => $"{IsPharmacy_MorningDose}-{IsPharmacy_AfternoonDose}-{IsPharmacy_EveningDose}";
        }

        public string IsPharmacy_MorningDose
        {
            get => isPharmacy_MorningDose;
            set => SetProperty(ref isPharmacy_MorningDose, value);
        }
        public string IsPharmacy_AfternoonDose
        {
            get => isPharmacy_AfternoonDose;
            set => SetProperty(ref isPharmacy_AfternoonDose, value);
        }
        public string IsPharmacy_EveningDose
        {
            get => isPharmacy_EveningDose;
            set => SetProperty(ref isPharmacy_EveningDose, value);
        }
        public string IsPharmacy_Note
        {
            get => isPharmacy_Note;
            set => SetProperty(ref isPharmacy_Note, value);
        }
        public int UserId { get; set; }

        public bool IsAllergyAnamnesis { get; set; }
        public string IsAllergy_Name { get; set; }
        public bool IsFamily_HeartAttack
        {
            get => isFamily_HeartAttack;
            set => SetProperty(ref isFamily_HeartAttack, value);
        }
        public bool IsFamilyAnamnesis 
        {
            get => isFamilyAnamnesis;
            set => SetProperty(ref isFamilyAnamnesis, value); 
        }
        public bool IsFamily_ICHS
        {
            get => isFamily_ICHS;
            set => SetProperty(ref isFamily_ICHS, value);
        }
        public bool IsFamily_ValveDefect
        {
            get => isFamily_ValveDefect;
            set => SetProperty(ref isFamily_ValveDefect, value);
        }
        public bool IsFamily_AtrialFibrillation
        {
            get => isFamily_AtrialFibrillation;
            set => SetProperty(ref isFamily_AtrialFibrillation, value);
        }
        public bool IsFamily_SuddenDeath
        {
            get => isFamily_SuddenDeath;
            set => SetProperty(ref isFamily_SuddenDeath, value);
        }
        public bool IsFamily_Pacemaker
        {
            get => isFamily_Pacemaker;
            set => SetProperty(ref isFamily_Pacemaker, value);
        }

        public bool IsAbususAnamnesis
        {
            get => isAbususAnamnesis;
            set => SetProperty(ref isAbususAnamnesis, value);
        }
        public bool IsAbusus_Alcohol
        {
            get => isAbusus_Alcohol;
            set => SetProperty(ref isAbusus_Alcohol, value);
        }
        public bool IsAbusus_Exsmoker
        {
            get => isAbusus_Exsmoker;
            set => SetProperty(ref isAbusus_Exsmoker, value);
        }
        public bool IsAbusus_Smoker
        {
            get => isAbusus_Smoker;
            set => SetProperty(ref isAbusus_Smoker, value);
        }

        public bool IsSocialAnamnesis
        {
            get => isSocialAnamnesis;
            set => SetProperty(ref isSocialAnamnesis, value);
        }

        public bool IsSocial_LivingWithPartner
        {
            get => isSocial_LivingWithPartner;
            set => SetProperty(ref isSocial_LivingWithPartner, value);
        }
        public bool IsSocial_Working
        {
            get => isSocial_Working;
            set => SetProperty(ref isSocial_Working, value);
        }
        public bool IsSocial_Pension
        {
            get => isSocial_Pension;
            set => SetProperty(ref isSocial_Pension, value);
        }
        public bool IsSocial_PartialDisabilityPension
        {
            get => isSocial_PartialDisabilityPension;
            set => SetProperty(ref isSocial_PartialDisabilityPension, value);
        }
        public bool IsSocial_DisabilityPension
        {
            get => isSocial_DisabilityPension;
            set => SetProperty(ref isSocial_DisabilityPension, value);
        }

        public bool IsPersonalAnamnesis { get; set; }
        public int IsPersonal_Type { get; set; }
        public DateTime IsPersonal_Date
        {
            get => isPersonal_Date;
            set => SetProperty(ref isPersonal_Date, value);
        }
        public string IsPersonal_Value
        {
            get => isPersonal_Value;
            set => SetProperty(ref isPersonal_Value, value);
        }
        public PhysicalActivityFrequencyType IsGeneral_PhysicalActivityFrequencyType
        {
            get => isGeneral_PhysicalActivityFrequencyType;
            set => SetProperty(ref isGeneral_PhysicalActivityFrequencyType, value);
        }

        public bool ShowDosageDetail
        {
            get => showDosageDetail;
            set => SetProperty(ref showDosageDetail, value);
        }
        public DateTime CreatedAt { get; set; }
        public bool IsLastRow { get; set; }

        public UserAnamnesisViewModel(UserAnamnesisDTO dto)
        {
            Id = dto.Id;
            UserId = dto.UserId;

            IsPharmacy_PharmacyId = dto.IsPharmacy_PharmacyId;
            IsPharmacy_Name = dto.IsPharmacy_Name;
            IsPharmacy_Dose = dto.IsPharmacy_Dose;
            IsPharmacy_MorningDose = dto.IsPharmacy_MorningDose;
            IsPharmacy_AfternoonDose = dto.IsPharmacy_AfternoonDose;
            IsPharmacy_EveningDose = dto.IsPharmacy_EveningDose;
            IsPharmacy_Note = dto.IsPharmacy_Note;

            IsAllergyAnamnesis = dto.IsAllergyAnamnesis;
            IsAllergy_Name = dto.IsAllergy_Name;

            IsPersonalAnamnesis = dto.IsPersonalAnamnesis;
            IsPersonal_Type = dto.IsPersonal_Type;
            IsPersonal_Date = dto.IsPersonal_Date;
            IsPersonal_Value = dto.IsPersonal_Value;

            IsGeneral_PhysicalActivityFrequencyType = dto.IsGeneral_PhysicalActivityFrequencyType;

            IsAbususAnamnesis = dto.IsAbususAnamnesis;
            IsAbusus_Alcohol = dto.IsAbusus_Alcohol;
            IsAbusus_Exsmoker = dto.IsAbusus_Exsmoker;
            IsAbusus_Smoker = dto.IsAbusus_Smoker;

            IsFamilyAnamnesis = dto.IsFamilyAnamnesis;
            IsFamily_HeartAttack = dto.IsFamily_HeartAttack;
            IsFamily_AtrialFibrillation = dto.IsFamily_AtrialFibrillation;
            IsFamily_ICHS = dto.IsFamily_ICHS;
            IsFamily_Pacemaker= dto.IsFamily_Pacemaker;
            IsFamily_SuddenDeath = dto.IsFamily_SuddenDeath;
            IsFamily_ValveDefect = dto.IsFamily_ValveDefect;

            IsSocialAnamnesis = dto.IsSocialAnamnesis;
            IsSocial_DisabilityPension = dto.IsSocial_DisabilityPension;
            IsSocial_LivingWithPartner= dto.IsSocial_LivingWithPartner;
            IsSocial_Pension = dto.IsSocial_Pension;
            IsSocial_PartialDisabilityPension = dto.IsSocial_PartialDisabilityPension;
            IsSocial_Working = dto.IsSocial_Working;

            CreatedAt = dto.CreatedAt;

        }
        public UserAnamnesisViewModel(PharmacyDTO dto)
        {
            IsPharmacy_PharmacyId = dto.Id;
            IsPharmacy_Name = dto.Name;
            IsPharmacy_Dose = dto.Supplement;
        }
        public UserAnamnesisViewModel()
        {

        }

        public object Clone()
        {
            return base.MemberwiseClone();
        }
    }
}
