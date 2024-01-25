namespace MyHeart.Services.Interfaces
{
    public interface IStripeConfiguration
    {
        string ApiKey { get; set; }
        string ApiSecret { get; set; }
        string SuccessUrl { get; set; }
        string CancelledUrl { get; set; }
        string WebhookSecret { get; set; }
    }
}