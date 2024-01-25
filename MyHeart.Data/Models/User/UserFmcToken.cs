using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.Models
{
    public class UserFmcToken : BaseModel
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
    }
}
