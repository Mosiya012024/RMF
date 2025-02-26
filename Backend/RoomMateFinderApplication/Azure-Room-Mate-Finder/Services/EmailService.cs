using Azure_Room_Mate_Finder.Model;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Azure_Room_Mate_Finder.Services
{
    public class EmailService
    {
        private readonly SmtpSettings smtpSettings;
        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            this.smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(this.smtpSettings.Server, this.smtpSettings.Port))
            {
                client.Credentials = new NetworkCredential(this.smtpSettings.UserName, this.smtpSettings.Password);
                client.EnableSsl = this.smtpSettings.EnableSsl;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(this.smtpSettings.SenderEmail,this.smtpSettings.SenderName),
                    Subject = subject,
                    Body = body, 
                    IsBodyHtml = true,
                    
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
             
        }

    }
}
