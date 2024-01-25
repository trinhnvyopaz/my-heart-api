using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Data.Models
{
    public class PaymentLog : BaseModel
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public string CustomerId { get; set; }
        public string ChargeId { get; set; }
        public string InvoiceId { get; set; }
        public string PaymentMethod { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string State { get; set; }
        public string FailureReason { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}


