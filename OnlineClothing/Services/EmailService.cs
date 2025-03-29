
using System.Net.Mail;
using System.Net;
using DotNetEnv;

namespace OnlineClothing.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;
        public EmailService()
        {
            Env.Load();
            _smtpUser = Environment.GetEnvironmentVariable("SMTP_USER")
            ?? throw new ArgumentNullException("SMTP_USER not set in .env");
            _smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD")
            ?? throw new ArgumentNullException("SMTP_PASSWORD not set in .env");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var fromEmail = new MailAddress(_smtpUser, "Online Clothing Shop");
            var toEmailAddress = new MailAddress(toEmail);

            var mailMessage = new MailMessage(fromEmail, toEmailAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };

            try
            {
                using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                    smtpClient.EnableSsl = true;
                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending email", ex);
            }
        }
    }
}
