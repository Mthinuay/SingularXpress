using System.Threading.Tasks;

namespace SingularExpress.Interfaces
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string otp, string username);
        Task SendPasswordResetConfirmationEmailAsync(string toEmail, string username);


    }
}