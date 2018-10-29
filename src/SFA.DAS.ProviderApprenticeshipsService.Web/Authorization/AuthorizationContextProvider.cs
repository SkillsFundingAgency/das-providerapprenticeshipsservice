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

        public AuthorizationContextProvider(HttpContextBase httpContext, IPublicHashingService publicHashingService)
        {
            _httpContext = httpContext;
            _publicHashingService = publicHashingService;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            //todo: change GetAuthorizationContext to Populate(AuthorizationContext ), or IAuthContext Get(authContext) as all will create one anyway? or derived??

            var routeValueDictionary = _httpContext.Request.RequestContext.RouteData.Values;

            //authorizationContext.Set(AuthorizationContextKeys.AccountLegalEntityId, GetAccountLegalEntityId());
            // authorizationContext.Set(AuthorizationContextKeys.ProviderId, GetProviderId());

            //var accountLegalEntityPublicHashedId = (string)routeValueDictionary[RouteDataKeys.AccountLegalEntityPublicHashedId];
            //var accountLegalEntityId = accountLegalEntityPublicHashedId != null ? _publicHashingService.DecodeValue(accountLegalEntityPublicHashedId) : (long?)null;
            //var accountLegalEntityId = _publicHashingService?.DecodeValue(accountLegalEntityPublicHashedId);

            //use initializer
            // replace tryget with [] in authcontext
            var authorizationContext = new AuthorizationContext();

            authorizationContext.Set("AccountLegalEntityId", GetAccountLegalEntityId(routeValueDictionary));
            // alternative source:
            // var providerId = int.Parse(User.Identity.GetClaim("http://schemas.portal.com/ukprn"));
            authorizationContext.Set("ProviderId", routeValueDictionary[RouteDataKeys.ProviderId]);

            return authorizationContext;

            // look for providerid
            // look for AccountLegalEntityPublicHashedId / AccountLegalEntityId directly
            // if not found, look for HashedCommitmentId / CommitmentId and fetch AccountLegalEntityPublicHashedIdFrom db
            // unhash along the way if necessary
            // bit nasty going to db every time
            // does this get called for every action?
            // in provider permissions case, we only actually need the context if a provider permission has been set on the DasAuthorizeAttribute, otherwise we should avoid getting the context
        }

        private long? GetAccountLegalEntityId(RouteValueDictionary routeValueDictionary)
        {
            var accountLegalEntityPublicHashedId = (string)routeValueDictionary[RouteDataKeys.AccountLegalEntityPublicHashedId];
            return accountLegalEntityPublicHashedId != null ? _publicHashingService.DecodeValue(accountLegalEntityPublicHashedId) : (long?)null;
        }
    }
}