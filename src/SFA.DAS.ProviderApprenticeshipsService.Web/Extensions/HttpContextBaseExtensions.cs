using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions
{
    public static class HttpContextBaseExtensions
    {
        public static string GetClaimValue(this HttpContext httpContext, string claimKey)
        {
            var claimIdentity = ((ClaimsIdentity)httpContext.User.Identity).Claims.FirstOrDefault(claim => claim.Type == claimKey);
            return claimIdentity != null ? claimIdentity.Value : string.Empty;
        }

        public static bool HasAnyClaimValue(this HttpContext httpContext, string claimType, string[] values)
        {
            return
                ((ClaimsIdentity)httpContext.User.Identity).Claims.Any(
                    x => x.Type == claimType && values.Contains(x.Value));
        }
    }
}