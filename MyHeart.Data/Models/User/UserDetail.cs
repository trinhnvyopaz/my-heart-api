using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyHeart.Data.Models
{
    public class UserDetail : BaseModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string PIN { get; set; }
        [Required]
        public int DoctorId { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public int InsuranceCompanyCode { get; set; }
        public int InsuranceNumber { get; set; }
        [DefaultValue(1)]
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
        public DateTime? SubscriptionValidTo { get; set; }
    }
}