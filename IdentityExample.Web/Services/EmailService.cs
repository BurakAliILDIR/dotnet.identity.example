using System.Net;
using System.Net.Mail;
using IdentityExample.Web.Settings;
using Microsoft.Extensions.Options;

namespace IdentityExample.Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendResetPasswordEmail(string passwordResetLink, string toEmail)
        {
            var mailMessage = new MailMessage();

            var smptClient = new SmtpClient
            {
                Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password),
                DeliveryFormat = SmtpDeliveryFormat.SevenBit,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true,
                Host = _emailSettings.Host,
                PickupDirectoryLocation = null,
                Port = 587,
                TargetName = null,
                Timeout = 0,
                UseDefaultCredentials = false
            };

            mailMessage.From = new MailAddress(_emailSettings.Email);
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = "Localhost | Şifre sıfırlama linki";
            mailMessage.Body = @$"<h4> Şifrenizi yenilemek için aşağıdaki linke tıklayınız.</h4>
                                <p>
                                    <a href='{passwordResetLink}'>Şifre Sıfırlama Linki</a>
                                </p>";

            mailMessage.IsBodyHtml = true;

            await smptClient.SendMailAsync(mailMessage);
        }
    }
}