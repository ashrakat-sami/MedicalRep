using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace MedicalRep.Models
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (var client = new SmtpClient())
            {
                client.Host = _emailSettings.SmtpServer;
                client.Port = _emailSettings.Port;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(
                    _emailSettings.Username,
                    _emailSettings.Password
                );

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.Username),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                try
                {
                    await client.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    // Log the error
                    Console.WriteLine($"Error sending email: {ex.Message}");
                    throw;
                }
            }
        }
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}