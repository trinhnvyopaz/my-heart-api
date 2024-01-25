using MyHeart.DTO.ProfessionInformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.Models.ProfessionInformation
{
    public class Atc : BaseModel
    {
        public string AtcCode { get; set; }
        public string NT { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public ICollection<MedicamentGroup_Atc> MedicamentGroups { get; set; }

        public int GetCodeHash() {
            return AtcDTO.GetAtcCodeHash(AtcCode);
        }
    }
}