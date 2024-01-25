using MyHeart.DTO.User;
using System.ComponentModel.DataAnnotations;

namespace MyHeart.Data.Models
{
    public class Role : BaseModel
    {
        [Required]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public UType RoleType { get; set; }
    }
}
