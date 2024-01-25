using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendMail(string to, string subject, string body);
        Task SendAdminMail(string subject, string body);
    }
}