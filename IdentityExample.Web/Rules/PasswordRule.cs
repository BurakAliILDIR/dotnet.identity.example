using IdentityExample.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityExample.Web.Rules
{
    public class PasswordRule : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
        {
            var errors = new List<IdentityError>();

            if (password!.ToLower().Contains(user.UserName!.ToLower()))
            {
                errors.Add(new IdentityError()
                {
                    Code = "PasswordContainUserName", Description = "Şifre alanı kullanıcı adı içeremez."
                });
            }

            if (password.ToLower().StartsWith("123"))
            {
                errors.Add(new IdentityError()
                {
                    Code = "PasswordStartWith123",
                    Description = "Şifre ardışık sayı ile başlayamaz."
                });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}