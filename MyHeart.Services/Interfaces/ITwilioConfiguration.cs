namespace MyHeart.Services.Interfaces
{
    public interface ITwilioConfiguration
    {
        string AccountSID { get; set; }
        string ApiKeySecret { get; set; }
        string ApiKeySID { get; set; }
        string AuthToken { get; set; }
    }
}