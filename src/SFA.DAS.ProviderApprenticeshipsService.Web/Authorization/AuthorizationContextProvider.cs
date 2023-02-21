using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.ProviderFeatures.Context;
using SFA.DAS.Authorization.ProviderPermissions.Context;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Routing;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization
{
    public class AuthorizationContextProvider : IAuthorizationContextProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountLegalEntityPublicHashingService _accountLegalEntityPublicHashingService;
        private readonly ILogger<AuthorizationContextProvider> _log;
        private readonly IActionContextAccessorWrapper _actionContextAccessorWrapper;

        public AuthorizationContextProvider(IHttpContextAccessor httpContextAccessor,
            IAccountLegalEntityPublicHashingService accountLegalEntityPublicHashingService,
            ILogger<AuthorizationContextProvider> log,
            IActionContextAccessorWrapper actionContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _accountLegalEntityPublicHashingService = accountLegalEntityPublicHashingService;
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
                return _accountLegalEntityPublicHashingService.DecodeValue(accountLegalEntityPublicHashedId);
                
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Unable to extract AccountLegalEntityId");
            }

            return null;
        }

        private long GetProviderId(RouteValueDictionary routeValueDictionary)
        {
            long providerId;

            if (long.TryParse(_httpContextAccessor.HttpContext.User.Identity.GetClaim(DasClaimTypes.Ukprn), out providerId))
                return providerId;

            if (long.TryParse((string) routeValueDictionary[RouteDataKeys.ProviderId], out providerId))
                return providerId;

            throw new Exception("AuthorizationContextProvider error - Unable to extract ProviderId");
        }

        private string GetUserEmail()
        {
            return _httpContextAccessor.HttpContext.User.Identity.GetClaim(DasClaimTypes.Email);
        }
    }
}