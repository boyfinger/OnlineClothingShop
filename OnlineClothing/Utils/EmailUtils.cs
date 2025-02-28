using OnlineClothing.Models;
using System.Net.Mail;
using System.Net;

namespace OnlineClothing.Utils
{
    public class EmailUtils
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "se1823.swp391.group4@gmail.com";
        private readonly string _smtpPassword = "zixr gapz hnye arch";

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
