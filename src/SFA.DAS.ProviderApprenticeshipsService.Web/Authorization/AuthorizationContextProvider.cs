using SFA.DAS.Encoding;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Routing;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;

public class AuthorizationContextProvider(
    IHttpContextAccessor httpContextAccessor,
    IEncodingService encodingService,
    ILogger<AuthorizationContextProvider> log,
    IActionContextAccessorWrapper actionContextAccessor)
    : IAuthorizationContextProvider
{
    public IAuthorizationContext GetAuthorizationContext()
    {
        var authorizationContext = new AuthorizationContext();
            
        var routeData = actionContextAccessor.GetRouteData();
        var ukprn = GetProviderId(routeData.Values);
        var accountLegalEntityId = GetAccountLegalEntityId(httpContextAccessor.HttpContext.Request.Query[RouteDataKeys.EmployerAccountLegalEntityPublicHashedId]);
        if (accountLegalEntityId != null)
        {
            authorizationContext.AddProviderPermissionValues(accountLegalEntityId.Value, ukprn);

        }

        var userEmail = GetUserEmail();
        if (userEmail != null)
        {
            authorizationContext.AddProviderFeatureValues(ukprn, userEmail);
        }

        return authorizationContext;
    }

    private long? GetAccountLegalEntityId(string employerAccountLegalEntityPublicHashedId)
    {
        try
        {
            var accountLegalEntityPublicHashedId = employerAccountLegalEntityPublicHashedId;

            if(accountLegalEntityPublicHashedId == null)
            {
                return null;
            }
            return encodingService.Decode(accountLegalEntityPublicHashedId,EncodingType.PublicAccountLegalEntityId);
                
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "Unable to extract AccountLegalEntityId");
        }
        
        return null;
    }

    private long GetProviderId(RouteValueDictionary routeValueDictionary)
    {
        if (long.TryParse(httpContextAccessor.HttpContext.User.Identity.GetClaim(DasClaimTypes.Ukprn), out var providerId))
        {
            return providerId;
        }

        if (long.TryParse((string) routeValueDictionary[RouteDataKeys.ProviderId], out providerId))
        {
            return providerId;
        }

        throw new InvalidOperationException("AuthorizationContextProvider error - Unable to extract ProviderId");
    }

    private string GetUserEmail()
    {
        return httpContextAccessor.HttpContext.User.Identity.GetClaim(DasClaimTypes.Email) ?? httpContextAccessor.HttpContext.User.Identity.GetClaim(DasClaimTypes.DfEEmail);
    }
}