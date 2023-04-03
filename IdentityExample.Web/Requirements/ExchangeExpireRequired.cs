using Microsoft.AspNetCore.Authorization;

namespace IdentityExample.Web.Requirements
{
    public class ExchangeExpireRequired : IAuthorizationRequirement
    {
    }

    public class ExchangeExpireRequirementHandler : AuthorizationHandler<ExchangeExpireRequired>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ExchangeExpireRequired requirement)
        {
            var hasExchangeExpireClaim = context.User.HasClaim(x => x.Type == "ExchangeExpireDate");

            if (!hasExchangeExpireClaim)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var exchangeExpireDate = context.User.FindFirst("ExchangeExpireDate");

            if (DateTime.UtcNow < Convert.ToDateTime(exchangeExpireDate.Value))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}