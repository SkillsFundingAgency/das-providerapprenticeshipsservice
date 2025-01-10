namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

public static class ClaimsExtensions
{
    public static string GetUserRef(this ClaimsPrincipal principal)
    {
       return principal.Identity.GetClaim(DasClaimTypes.Upn) ?? principal.Identity.GetClaim("sub");
    }
}