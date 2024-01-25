namespace MyHeart.Services.Interfaces
{
    public interface IEmailConfiguration
    {
        string SmtpPassword { get; set; }
        int SmtpPort { get; set; }
        string SmtpServer { get; set; }
        string SmtpUsername { get; set; }
        bool UseSSL { get; set; }
        string AdminEmail { get; set; }
        string From { get; set; }
    }
}