using System.Threading.Tasks;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
