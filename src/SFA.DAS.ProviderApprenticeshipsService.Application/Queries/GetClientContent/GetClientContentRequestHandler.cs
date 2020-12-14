using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetClientContent
{
    public class GetClientContentRequestHandler : IRequestHandler<GetClientContentRequest, GetClientContentResponse>
    {           
        private readonly IClientContentService _service;
        private readonly ICacheStorageService _cacheStorageService;
        private readonly ILog _logger;
        private readonly ProviderApprenticeshipsServiceConfiguration _providerApprenticeshipsServiceConfiguration;

        public GetClientContentRequestHandler(         
           ILog logger,
           IClientContentService service,
           ICacheStorageService cacheStorageService,
           ProviderApprenticeshipsServiceConfiguration providerApprenticeshipsServiceConfiguration)
        {   _logger = logger;
            _service = service;
            _cacheStorageService = cacheStorageService;
            _providerApprenticeshipsServiceConfiguration = providerApprenticeshipsServiceConfiguration;
        }

        public async  Task<GetClientContentResponse> Handle(GetClientContentRequest request, CancellationToken cancellationToken)
        {
            var applicationId = request.UseLegacyStyles ? _providerApprenticeshipsServiceConfiguration.ContentApplicationId + "-legacy" : _providerApprenticeshipsServiceConfiguration.ContentApplicationId;
            var cacheKey = $"{applicationId}_{request.ContentType}".ToLowerInvariant();

            try
            {              
                if (_cacheStorageService.TryGet(cacheKey, out string cachedContentBanner))
                {
                    return new GetClientContentResponse
                    {
                        Content = cachedContentBanner
                    };
                }

                var contentBanner = await _service.Get(request.ContentType, applicationId);

                if (contentBanner != null)
                {
                    await _cacheStorageService.Save(cacheKey, contentBanner, 1);
                }
                return new GetClientContentResponse
                {
                    Content = contentBanner
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to get Content for {cacheKey}");

                return new GetClientContentResponse
                {
                    HasFailed = true
                };
            }
        }
    }
}
