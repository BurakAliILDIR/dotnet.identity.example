using IdentityExample.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityExample.Web.Rules
{
    public class UserRule : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var errors = new List<IdentityError>();

            var isNumeric = int.TryParse(user.UserName[0].ToString(), out _);

            if (isNumeric)
            {
                errors.Add(new IdentityError()
                {
                    Code = "UserNameStartWithNumeric", Description = "Kullanıcı adı sayıyla başlayamaz."
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