namespace IdentityExample.Web.Services
{
    public interface IEmailService
    {
        Task SendResetPasswordEmail(string passwordResetLink, string toEmail);
    }
}