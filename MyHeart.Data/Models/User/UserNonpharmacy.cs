using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.Models
{
    public class UserNonpharmacy : BaseModel
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int NonpharmaticTherapyId { get; set; }
        public NonpharmaticTherapy NonpharmaticTherapy { get; set; }

        public string FacilityName { get; set; }
        public string Note { get; set; }
    }
}
