using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.ProviderFeatures.Context;
using SFA.DAS.Authorization.ProviderPermissions.Context;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Routing;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;

public class AuthorizationContextProvider : IAuthorizationContextProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEncodingService _encodingService;
    private readonly ILogger<AuthorizationContextProvider> _log;
    private readonly IActionContextAccessorWrapper _actionContextAccessorWrapper;

    public AuthorizationContextProvider(IHttpContextAccessor httpContextAccessor,
        IEncodingService encodingService,
        ILogger<AuthorizationContextProvider> log,
        IActionContextAccessorWrapper actionContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _encodingService = encodingService;
        _log = log;
        _actionContextAccessorWrapper = actionContextAccessor;
    }

    public IAuthorizationContext GetAuthorizationContext()
    {
        var authorizationContext = new AuthorizationContext();
            
        var routeData = _actionContextAccessorWrapper.GetRouteData();
        var ukprn = GetProviderId(routeData.Values);
        var accountLegalEntityId = GetAccountLegalEntityId(_httpContextAccessor.HttpContext.Request.Query[RouteDataKeys.EmployerAccountLegalEntityPublicHashedId]);
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
            return _encodingService.Decode(accountLegalEntityPublicHashedId,EncodingType.PublicAccountLegalEntityId);
                
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "Unable to extract AccountLegalEntityId");
        }
        
        return null;
    }

    private long GetProviderId(RouteValueDictionary routeValueDictionary)
    {
        if (long.TryParse(_httpContextAccessor.HttpContext.User.Identity.GetClaim(DasClaimTypes.Ukprn), out var providerId))
            return providerId;

        if (long.TryParse((string) routeValueDictionary[RouteDataKeys.ProviderId], out providerId))
            return providerId;

        throw new InvalidOperationException("AuthorizationContextProvider error - Unable to extract ProviderId");
    }

    private string GetUserEmail()
    {
        return _httpContextAccessor.HttpContext.User.Identity.GetClaim(DasClaimTypes.Email) ?? _httpContextAccessor.HttpContext.User.Identity.GetClaim(DasClaimTypes.DfEEmail);
    }
}