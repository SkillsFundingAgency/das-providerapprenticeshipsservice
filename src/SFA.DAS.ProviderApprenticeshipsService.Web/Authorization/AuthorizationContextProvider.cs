using System;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.ProviderFeatures.Context;
using SFA.DAS.Authorization.ProviderPermissions.Context;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Routing;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization
{
    public class AuthorizationContextProvider : IAuthorizationContextProvider
    {
        private readonly HttpContext _httpContext;
        private readonly IAccountLegalEntityPublicHashingService _accountLegalEntityPublicHashingService;
        private readonly ILogger<AuthorizationContextProvider> _log;
        private readonly IActionContextAccessor _actionContextAccessor;

        public AuthorizationContextProvider(HttpContext httpContext,
            IAccountLegalEntityPublicHashingService accountLegalEntityPublicHashingService,
            ILogger<AuthorizationContextProvider> log,
            IActionContextAccessor actionContextAccessor)
        {
            _httpContext = httpContext;
            _accountLegalEntityPublicHashingService = accountLegalEntityPublicHashingService;
            _log = log;
            _actionContextAccessor = actionContextAccessor;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            var authorizationContext = new AuthorizationContext();
            
            var ukprn = GetProviderId(_actionContextAccessor.ActionContext.RouteData.Values);
            var accountLegalEntityId = GetAccountLegalEntityId(_httpContext.Request.Query[RouteDataKeys.EmployerAccountLegalEntityPublicHashedId]);
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

            if (long.TryParse(_httpContext.User.Identity.GetClaim(DasClaimTypes.Ukprn), out providerId))
                return providerId;

            if (long.TryParse((string) routeValueDictionary[RouteDataKeys.ProviderId], out providerId))
                return providerId;

            throw new Exception("AuthorizationContextProvider error - Unable to extract ProviderId");
        }

        private string GetUserEmail()
        {
            return _httpContext.User.Identity.GetClaim(DasClaimTypes.Email);
        }
    }
}