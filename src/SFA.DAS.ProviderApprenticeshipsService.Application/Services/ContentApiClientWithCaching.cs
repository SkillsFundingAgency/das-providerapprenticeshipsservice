using System;
using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Services
{
    public class ContentApiClientWithCaching : IContentApiClient
    {
        private readonly IContentApiClient _contentService;
        private readonly ICacheStorageService _cacheStorageService;
        private readonly ProviderApprenticeshipsServiceConfiguration _providerApprenticeshipServiceConfiguration;

        public ContentApiClientWithCaching(IContentApiClient contentService, ICacheStorageService cacheStorageService, ProviderApprenticeshipsServiceConfiguration providerApprenticeshipServiceConfiguration)
        {
            _contentService = contentService;
            _cacheStorageService = cacheStorageService;
            _providerApprenticeshipServiceConfiguration = providerApprenticeshipServiceConfiguration;
        }

        public async Task<string> Get(string type, string applicationId)
        {
            var cacheKey = $"{applicationId}_{type}".ToLowerInvariant();

            if (_cacheStorageService.TryGet(cacheKey, out string cachedContentBanner))
            {
                return cachedContentBanner;
            }

            var content = await _contentService.Get(type, applicationId);

            if (content != null)
            {
                await _cacheStorageService.Save(cacheKey, content, _providerApprenticeshipServiceConfiguration.DefaultCacheExpirationInMinutes);
            }

            return content;
        }
    }
}
