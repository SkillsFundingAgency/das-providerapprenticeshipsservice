using Microsoft.Extensions.Caching.Memory;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Caching;

public interface IAuthorizationResultCacheConfigurationProvider
{
    Type HandlerType { get; }
        
    object GetCacheKey(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext);
    void ConfigureCacheEntry(ICacheEntry cacheEntry);
}