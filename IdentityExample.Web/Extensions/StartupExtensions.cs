using IdentityExample.Web.Localizations;
using IdentityExample.Web.Models;
using IdentityExample.Web.Rules;

namespace IdentityExample.Web.Extensions
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithExtension(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;

                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;

                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                    options.Lockout.MaxFailedAccessAttempts = 3;
                })
                .AddPasswordValidator<PasswordRule>()
                .AddUserValidator<UserRule>()
                .AddErrorDescriber<IdentityErrorDescriberLocalize>()
                .AddEntityFrameworkStores<AppDbContext>();
        }
    }
}