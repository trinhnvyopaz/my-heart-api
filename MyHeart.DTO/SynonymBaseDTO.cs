using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyHeart.DTO
{
    public class SynonymBaseDTO : BaseEntityDTO
    {
        public string Name { get; set; }
        public bool IsManual { get; set; }
    }
}
