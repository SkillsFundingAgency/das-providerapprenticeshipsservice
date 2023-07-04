namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

public static class HttpContextExtensions
{
    public static string GetClaimValue(this HttpContext httpContext, string claimKey)
    {
        var claimIdentity = ((ClaimsIdentity)httpContext.User.Identity).Claims.FirstOrDefault(claim => claim.Type == claimKey);
        return claimIdentity != null ? claimIdentity.Value : string.Empty;
    }
}