using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions
{
    public static class IIdentityExtensions
    {
        public static bool HasServiceClaim(this IIdentity identity)
        {
            var claimsPrincipal = new ClaimsPrincipal(identity);
            return claimsPrincipal.FindAll(c => c.Type == DasClaimTypes.Service).Any();
        }

        public static string GetClaim(this IIdentity identity, string claim)
        {
            var claimsPrincipal = new ClaimsPrincipal(identity);
            return claimsPrincipal.FindFirst(c => c.Type == claim)?.Value;
        }
    }
}
