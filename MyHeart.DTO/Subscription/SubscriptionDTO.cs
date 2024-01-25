using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO
{
    public class SubscriptionDTO : BaseEntityDTO
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ProductStripeId { get; set; }
        public bool CancelAtPeriodEnd { get; set; }
    }
}
