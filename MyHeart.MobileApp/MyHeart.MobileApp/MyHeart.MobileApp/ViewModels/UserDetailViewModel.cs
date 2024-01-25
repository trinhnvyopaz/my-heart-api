using MyHeart.DTO;
using MyHeart.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.MobileApp.ViewModels
{
    public class UserDetailViewModel : BaseViewModel
    {
        private string phone;
        private string street;
        private string streetNumber;
        private string city;
        private string zip;
        private string firstName;
        private string lastName;
        private string email;
        private string pin;
        private int insuranceCompanyCode;
        private int insuranceNumber;

        public int Id { get; set; }
        public string FirstName
        {
            get => firstName;
            set => SetProperty(ref firstName, value);
        }
        public string LastName
        {
            get => lastName;
            set => SetProperty(ref lastName, value);
        }
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string Phone
        {
            get => phone;
            set => SetProperty(ref phone, value);
        }
        public string Street
        {
            get => street;
            set => SetProperty(ref street, value);
        }
        public string StreetNumber
        {
            get => streetNumber;
            set => SetProperty(ref streetNumber, value);
        }
        public string City
        {
            get => city;
            set => SetProperty(ref city, value);
        }
        public string Zip
        {
            get => zip;
            set => SetProperty(ref zip, value);
        }
        public string PIN
        {
            get => pin;
            set => SetProperty(ref pin, value);
        }

        public int InsuranceCompanyCode
        {
            get => insuranceCompanyCode;
            set => SetProperty(ref insuranceCompanyCode, value);
        }
        public int InsuranceNumber
        {
            get => insuranceNumber;
            set => SetProperty(ref insuranceNumber, value);
        }
        public int DoctorId { get; set; }
        public int SubscriptionId { get; set; }
        public SubscriptionDTO Subscription { get; set; }
        public int UserId { get; set; }

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

        public List<UserPersonalDiseaseDTO> Personal_Diseases { get; set; }
        public List<UserPersonalNonpharmaticDTO> Personal_NonpharmaticId { get; set; }

        public List<UserPharmacyDTO> Pharmacies { get; set; }

        public string IsAllergy_Name { get; set; }

        public bool IsAbusus_Alcohol { get; set; }
        public bool IsAbusus_Exsmoker { get; set; }
        public bool IsAbusus_Smoker { get; set; }

        public bool IsSocial_LivingWithPartner { get; set; }
        public bool IsSocial_Working { get; set; }
        public bool IsSocial_Pension { get; set; }
        public bool IsSocial_PartialDisabilityPension { get; set; }
        public bool IsSocial_DisabilityPension { get; set; }

        public UserDetailViewModel(UserDetailDTO dto)
        {
            Id = dto.Id;
            FirstName = dto.FirstName;
            LastName = dto.LastName;
            Email = dto.Email;
            Phone = dto.Phone;
            Street = dto.Street;
            StreetNumber = dto.StreetNumber;
            City = dto.City;
            Zip = dto.Zip;
            PIN = dto.PIN;
            DoctorId = dto.DoctorId;
            SubscriptionId = dto.SubscriptionId;
            Subscription = dto.Subscription;
            UserId = dto.UserId;
            IsFamilyAnamnesis = dto.IsFamilyAnamnesis;
            IsPersonalAnamnesis = dto.IsPersonalAnamnesis;
            IsPharmacyAnamnesis = dto.IsPharmacyAnamnesis;
            IsAllergyAnamnesis = dto.IsAllergyAnamnesis;
            IsAbususAnamnesis = dto.IsAbususAnamnesis;
            IsSocialAnamnesis = dto.IsSocialAnamnesis;
            IsFamily_ICHS = dto.IsFamily_ICHS;
            IsFamily_ValveDefect = dto.IsFamily_ValveDefect;
            IsFamily_AtrialFibrillation = dto.IsFamily_AtrialFibrillation;
            IsFamily_SuddenDeath = dto.IsFamily_SuddenDeath;
            IsFamily_Pacemaker = dto.IsFamily_Pacemaker;
            Personal_Diseases = dto.Personal_Diseases;
            Personal_NonpharmaticId = dto.Personal_NonpharmaticId;
            Pharmacies = dto.Pharmacies;
            IsAllergy_Name = dto.IsAllergy_Name;
            IsAbusus_Alcohol = dto.IsAbusus_Alcohol;
            IsAbusus_Exsmoker = dto.IsAbusus_Exsmoker;
            IsAbusus_Smoker = dto.IsAbusus_Smoker;
            IsSocial_LivingWithPartner = dto.IsSocial_LivingWithPartner;
            IsSocial_Working = dto.IsSocial_Working;
            IsSocial_Pension = dto.IsSocial_Pension;
            IsSocial_PartialDisabilityPension = dto.IsSocial_PartialDisabilityPension;
            IsSocial_DisabilityPension = dto.IsSocial_DisabilityPension;
            InsuranceCompanyCode = dto.InsuranceCompanyCode;
            InsuranceNumber = dto.InsuranceNumber;
        }
    }
}
