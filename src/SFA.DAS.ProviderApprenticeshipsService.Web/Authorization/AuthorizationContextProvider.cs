using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Routing;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.ProviderFeatures.Context;
using SFA.DAS.Authorization.ProviderPermissions.Context;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Routing;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization
{
    public class AuthorizationContextProvider : IAuthorizationContextProvider
    {
        private readonly HttpContextBase _httpContext;
        private readonly IAccountLegalEntityPublicHashingService _accountLegalEntityPublicHashingService;
        private readonly ILog _log;

        public AuthorizationContextProvider(HttpContextBase httpContext, IAccountLegalEntityPublicHashingService accountLegalEntityPublicHashingService, ILog log)
        {
            _httpContext = httpContext;
            _accountLegalEntityPublicHashingService = accountLegalEntityPublicHashingService;
            _log = log;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            var request = _httpContext.Request;

            var authorizationContext = new AuthorizationContext();
            
            var ukprn = GetProviderId(request.RequestContext.RouteData.Values); // alternative source: long.Parse(User.Identity.GetClaim("http://schemas.portal.com/ukprn"));
            var accountLegalEntityId = GetAccountLegalEntityId(request.Params);
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

        private long? GetAccountLegalEntityId(NameValueCollection parameters)
        {
            try
            {
                var accountLegalEntityPublicHashedId = parameters[RouteDataKeys.EmployerAccountLegalEntityPublicHashedId];

                if(accountLegalEntityPublicHashedId == null)
                {
                    return null;
                }
                return _accountLegalEntityPublicHashingService.DecodeValue(accountLegalEntityPublicHashedId);
                
            }
            catch (Exception ex)
            {
                _log.Warn(ex, "Unable to extract AccountLegalEntityId");
            }

            return null;
        }

        private long GetProviderId(RouteValueDictionary routeValueDictionary)
        {
            long providerId;

            if (long.TryParse(_httpContext.User.Identity.GetClaim("http://schemas.portal.com/ukprn"), out providerId))
                return providerId;

            if (long.TryParse((string) routeValueDictionary[RouteDataKeys.ProviderId], out providerId))
                return providerId;

            throw new Exception("AuthorizationContextProvider error - Unable to extract ProviderId");
        }

        private string GetUserEmail()
        {
            return _httpContext.User.Identity.GetClaim("http://schemas.portal.com/mail");
        }
    }
}