using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO
{
    public class BaseEntityDTO
    {
        public int Id { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string LastUpdateUser { get; set; }
    }
}
