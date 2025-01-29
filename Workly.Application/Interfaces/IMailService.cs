using Workly.Application.DTOs.Email;

namespace Workly.Application.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailAsync(EmailModel emailModel);
        Task NotifySupportTeamAsync(Exception ex, string errorToEmail, string errorSubject);
    }
}
