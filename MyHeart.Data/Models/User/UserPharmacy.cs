using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHeart.Data.Models
{
    public class UserPharmacy : BaseModel
    {
        [Required]
        public string Name { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        [Required]
        public string Dosing { get; set; }
        public int? PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }
    }
}
