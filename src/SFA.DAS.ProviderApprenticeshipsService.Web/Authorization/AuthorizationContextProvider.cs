using System.Web;
using System.Web.Routing;
using SFA.DAS.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Routing;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization
{
    public class AuthorizationContextProvider : IAuthorizationContextProvider
    {
        private readonly HttpContextBase _httpContext;
        private readonly IPublicHashingService _publicHashingService;

        private static class Keys
        {
            public const string AccountLegalEntityId = "AccountLegalEntityId";
            public const string ProviderId = "ProviderId";
        }

        public AuthorizationContextProvider(HttpContextBase httpContext, IPublicHashingService publicHashingService)
        {
            _httpContext = httpContext;
            _publicHashingService = publicHashingService;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            var routeValueDictionary = _httpContext.Request.RequestContext.RouteData.Values;

            //todo: insert everything we calc into here... or use magic binding
            //_httpContext.Items["accountId"] = 123;

            //var authorizationContext = new AuthorizationContext();

            //authorizationContext.AddProviderPermissionsContext(
            //    GetProviderId(routeValueDictionary),
            //    GetAccountLegalEntityId(routeValueDictionary));

            //return authorizationContext;

            return new AuthorizationContext
            {
                { Keys.AccountLegalEntityId, GetAccountLegalEntityId(routeValueDictionary) },
                { Keys.ProviderId, GetProviderId(routeValueDictionary) }            // alternative source: long.Parse(User.Identity.GetClaim("http://schemas.portal.com/ukprn"));
            };
        }

        private long? GetAccountLegalEntityId(RouteValueDictionary routeValueDictionary)
        {
            var accountLegalEntityPublicHashedId = (string)routeValueDictionary[RouteDataKeys.AccountLegalEntityPublicHashedId];
            return accountLegalEntityPublicHashedId != null ? _publicHashingService.DecodeValue(accountLegalEntityPublicHashedId) : (long?)null;
        }

        private long? GetProviderId(RouteValueDictionary routeValueDictionary)
        {
            if (long.TryParse((string) routeValueDictionary[RouteDataKeys.ProviderId], out var providerId))
                return providerId;

            return null;
        }
    }
}