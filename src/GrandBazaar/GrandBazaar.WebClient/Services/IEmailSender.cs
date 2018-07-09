using System.Threading.Tasks;

namespace GrandBazaar.WebClient.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
