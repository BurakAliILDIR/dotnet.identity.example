using IdentityExample.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace IdentityExample.Web.ClaimProvides
{
    public class ClaimProvider : IClaimsTransformation
    {
        private readonly UserManager<AppUser> _userManager;

        public ClaimProvider(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identity as ClaimsIdentity;

            var user = await _userManager.FindByNameAsync(identity.Name);

            if (user.City.IsNullOrEmpty())
            {
                return principal;
            }

            if (!principal.HasClaim(x => x.Type == "city"))
            {
                Claim claim = new Claim("city", user.City);
                identity.AddClaim(claim);
            }

            return principal;
        }
    }
}