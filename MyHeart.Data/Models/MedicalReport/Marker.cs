using MyHeart.DTO.MedicalReport;
using System.ComponentModel.DataAnnotations;

namespace MyHeart.Data.Models
{
    public class Marker : BaseModel
    {
        [Required]
        public int ParameterId { get; set; }
        public MakerGroup MakerGroup { get; set; }
        public MakerType Type { get; set; }
        public string Value { get; set; }
        public virtual Parameter Parameter { get; set; }
    }
}
