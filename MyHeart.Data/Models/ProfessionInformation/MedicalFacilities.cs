using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class MedicalFacilities : BaseModel
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

        public ICollection<NonpharmaticTherapy_MedicalFacilities_MedicalIntervention> NonpharmaticTherapy { get; set; }
        public ICollection<MedicalFacilities_PreventiveMeasures_Preventive> PreventiveMeasures { get; set; }
    }
}
