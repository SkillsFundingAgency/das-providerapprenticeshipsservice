using System.Collections;
using System.Collections.Generic;
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
            //todo: change GetAuthorizationContext to Populate(AuthorizationContext ), or IAuthContext Get(authContext) as all will create one anyway? or derived??

            var routeValueDictionary = _httpContext.Request.RequestContext.RouteData.Values;

            return new AuthorizationContext
            {
                { Keys.AccountLegalEntityId, GetAccountLegalEntityId(routeValueDictionary) },
                { Keys.ProviderId, routeValueDictionary[RouteDataKeys.ProviderId] }            // alternative source: long.Parse(User.Identity.GetClaim("http://schemas.portal.com/ukprn"));
            };
        }

        private long? GetAccountLegalEntityId(RouteValueDictionary routeValueDictionary)
        {
            var accountLegalEntityPublicHashedId = (string)routeValueDictionary[RouteDataKeys.AccountLegalEntityPublicHashedId];
            return accountLegalEntityPublicHashedId != null ? _publicHashingService.DecodeValue(accountLegalEntityPublicHashedId) : (long?)null;
        }
    }

    public class AuthorizationContext : IAuthorizationContext, IEnumerable
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        public T Get<T>(string key)
        {
            if (_data.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            throw new KeyNotFoundException($"The key '{key}' was not present in the authorization context");
        }

        public void Set<T>(string key, T value)
        {
            _data.Add(key, value);
        }

        public void Add<T>(string key, T value)
        {
            _data.Add(key, value);
        }

        public bool TryGet<T>(string key, out T value)
        {
            var exists = _data.TryGetValue(key, out var obj);

            value = exists ? (T)obj : default(T);

            return exists;
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable) _data).GetEnumerator();
        }
    }
}