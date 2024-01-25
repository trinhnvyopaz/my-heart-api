using MyHeart.Services.Interfaces;

namespace MyHeart.Services
{
    public class EmailConfiguration : IEmailConfiguration
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool UseSSL { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string AdminEmail { get; set; }
        public string From { get; set; }
    }
}