using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO.ProfessionalInformation
{
    public class MedicalFacilitiesDTO : BaseEntityDTO
    {
        public int? FacilityId { get; set; }
        [Required]
        public string Name { get; set; }
        public string FacilityCode { get; set; }
        public int? FacilityTypeCode { get; set; }
        //druhZarizeni
        public string Character { get; set; }
        public string CharacterSecondary { get; set; }
        public int? ICO { get; set; }
        public int? PCZ { get; set; }
        public int? PCDP { get; set; }
        //kraj
        public string Region { get; set; }
        public string RegionCode { get; set; }
        //okres
        public string District { get; set; }
        public string DistrictCode { get; set; }
        //spravniObvod
        public string AdministrativeDistrict { get; set; }
        //obec
        public string Municipality { get; set; }
        public string ZIP { get; set; }
        public string Street { get; set; }
        //cisloPopisne
        public string BuildingNumber { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string ProviderType { get; set; }
        public string ProviderName { get; set; }
        //OborPece
        public string CareField { get; set; }
        public string CareForm { get; set; }
        public string CareType { get; set; }
        //OdbornyZastupce
        public string Representative { get; set; }
        public string GPS { get; set; }

        public List<NonpharmaticTherapy_MedicalFacilities_MedicalInterventionDTO> NonpharmaticTherapy { get; set; }
        public List<MedicalFacilities_PreventiveMeasures_PreventiveDTO> PreventiveMeasures { get; set; }

        public static MedicalFacilitiesDTO FromCsv(string csv) {
            var values = csv.Split(';');
            var facility = new MedicalFacilitiesDTO {
                FacilityId = int.Parse(values[0].Trim('"')),
                Name = values[3].Trim('"'),
                FacilityCode = values[4].Trim('"'),
                FacilityTypeCode = int.Parse(values[5].Trim('"')),
                Character = values[6].Trim('"'),
                CharacterSecondary = values[7].Trim('"'),
                ICO = int.Parse(values[25].Trim('"')),
                PCZ = int.Parse(values[1].Trim('"')),
                PCDP = int.Parse(values[2].Trim('"')),
                Region = values[12].Trim('"'),
                RegionCode = values[13].Trim('"'),
                District = values[14].Trim('"'),
                DistrictCode = values[15].Trim('"'),
                AdministrativeDistrict = values[16].Trim('"'),
                Municipality = values[8].Trim('"'),
                ZIP = values[9].Trim('"'),
                Street = values[10].Trim('"'),
                BuildingNumber = values[11].Trim('"'),
                Email = values[21].Trim('"'),
                Fax = values[18].Trim('"'),
                Telephone = values[17].Trim('"'),
                Website = values[22].Trim('"'),
                ProviderType = values[23].Trim('"'),
                ProviderName = values[24].Trim('"'),
                CareField = values[39].Trim('"'),
                CareForm = values[40].Trim('"'),
                CareType = values[41].Trim('"'),
                Representative = values[42].Trim('"'),
                GPS = values[43].Trim('"')
            };

            facility.Id = facility.FacilityId.Value;

            return facility;
        }

        public override bool Equals(object obj) {
            return obj is MedicalFacilitiesDTO d &&
                   this.Id == d.Id &&
                   this.LastUpdateDate == d.LastUpdateDate &&
                   this.LastUpdateUser == d.LastUpdateUser &&
                   this.FacilityId == d.FacilityId &&
                   this.Name == d.Name &&
                   this.FacilityCode == d.FacilityCode &&
                   this.FacilityTypeCode == d.FacilityTypeCode &&
                   this.Character == d.Character &&
                   this.CharacterSecondary == d.CharacterSecondary &&
                   this.ICO == d.ICO &&
                   this.PCZ == d.PCZ &&
                   this.PCDP == d.PCDP &&
                   this.Region == d.Region &&
                   this.RegionCode == d.RegionCode &&
                   this.District == d.District &&
                   this.DistrictCode == d.DistrictCode &&
                   this.AdministrativeDistrict == d.AdministrativeDistrict &&
                   this.Municipality == d.Municipality &&
                   this.ZIP == d.ZIP &&
                   this.Street == d.Street &&
                   this.BuildingNumber == d.BuildingNumber &&
                   this.Email == d.Email &&
                   this.Telephone == d.Telephone &&
                   this.Fax == d.Fax &&
                   this.Website == d.Website &&
                   this.ProviderType == d.ProviderType &&
                   this.ProviderName == d.ProviderName &&
                   this.CareField == d.CareField &&
                   this.CareForm == d.CareForm &&
                   this.CareType == d.CareType &&
                   this.Representative == d.Representative &&
                   this.GPS == d.GPS;
        }

        public bool EqualsWithoutBaseEntity(object obj) {
            return obj is MedicalFacilitiesDTO d &&
                   this.FacilityId == d.FacilityId &&
                   this.Name == d.Name &&
                   this.FacilityCode == d.FacilityCode &&
                   this.FacilityTypeCode == d.FacilityTypeCode &&
                   this.Character == d.Character &&
                   this.CharacterSecondary == d.CharacterSecondary &&
                   this.ICO == d.ICO &&
                   this.PCZ == d.PCZ &&
                   this.PCDP == d.PCDP &&
                   this.Region == d.Region &&
                   this.RegionCode == d.RegionCode &&
                   this.District == d.District &&
                   this.DistrictCode == d.DistrictCode &&
                   this.AdministrativeDistrict == d.AdministrativeDistrict &&
                   this.Municipality == d.Municipality &&
                   this.ZIP == d.ZIP &&
                   this.Street == d.Street &&
                   this.BuildingNumber == d.BuildingNumber &&
                   this.Email == d.Email &&
                   this.Telephone == d.Telephone &&
                   this.Fax == d.Fax &&
                   this.Website == d.Website &&
                   this.ProviderType == d.ProviderType &&
                   this.ProviderName == d.ProviderName &&
                   this.CareField == d.CareField &&
                   this.CareForm == d.CareForm &&
                   this.CareType == d.CareType &&
                   this.Representative == d.Representative &&
                   this.GPS == d.GPS;
        }
    }
}
