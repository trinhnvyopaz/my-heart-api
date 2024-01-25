using System;
using System.Collections.Generic;
using System.Text;
using MyHeart.Services.Interfaces;

namespace MyHeart.Services
{
    public class StripeConfiguration : IStripeConfiguration
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelledUrl { get; set; }
        public string WebhookSecret { get; set; }
    }
}
