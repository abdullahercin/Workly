using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Workly.Application.DTOs.Email;
using Workly.Application.Interfaces;
using Workly.Application.Models;

namespace Workly.Infrastructure.Services
{
    public class MailService(IOptions<SmtpSettings> options, ILogger<MailService> logger) : IMailService
    {
        private readonly SmtpSettings smtpSettings = options.Value;
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(smtpSettings.SenderName, smtpSettings.SenderEmail));
                email.To.Add(new MailboxAddress("Recipient Name", toEmail));
                email.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = message
                };

                email.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpSettings.Server, smtpSettings.Port, false);
                await client.AuthenticateAsync(smtpSettings.Username, smtpSettings.Password);
                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mail gönderiminde hata oluştu.");
                await NotifySupportTeamAsync(ex, toEmail, subject);
            }

        }
        public async Task NotifySupportTeamAsync(Exception ex, string errorToEmail, string errorSubject)
        {
            try
            {
                var supportEmail = "bilgiislem@karatas.web.tr";
                var subject = "Mail Gönderim Hatası";
                var body = $"Mail gönderiminde hata oluştu. Alıcı:{errorToEmail} | Konu:{errorSubject} | Hata mesajı: {ex.Message}";
                await SendEmailAsync(supportEmail, subject, body);
            }
            catch (Exception)
            {
                logger.LogError(ex, "Mail gönderiminde hata oluştu.");
            }
        }

        public async Task SendEmailAsync(EmailModel emailModel)
        {
            await SendEmailAsync(emailModel.To, emailModel.Subject, emailModel.Body);
        }
    }
}
