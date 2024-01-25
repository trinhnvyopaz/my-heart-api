using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO.ProfessionInformation.MedicamentGroupBonds
{
    public class MedicamentGroup_AtcDTO : BaseEntityDTO
    {
        public int MedicamentGroupId { get; set; }
        public int AtcId { get; set; }
    }
}
