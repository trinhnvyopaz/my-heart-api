using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.Models
{
    public class PasswordResetTicket : BaseModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool TokenUsed { get; set; }
    }
}
