using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetClientContent;

public class GetClientContentRequestHandler : IRequestHandler<GetClientContentRequest, GetClientContentResponse>
{           
    private readonly IContentApiClient _contentApiClient;
    private readonly ILogger<GetClientContentRequestHandler> _logger;
    private readonly ProviderApprenticeshipsServiceConfiguration _providerApprenticeshipsServiceConfiguration;

    public GetClientContentRequestHandler(
        ILogger<GetClientContentRequestHandler> logger,
        IContentApiClient contentApiClient,
        ProviderApprenticeshipsServiceConfiguration providerApprenticeshipsServiceConfiguration)
    {   _logger = logger;
        _contentApiClient = contentApiClient;
        _providerApprenticeshipsServiceConfiguration = providerApprenticeshipsServiceConfiguration;
    }

    public async Task<GetClientContentResponse> Handle(GetClientContentRequest request, CancellationToken cancellationToken)
    {
        var applicationId = request.UseLegacyStyles ? _providerApprenticeshipsServiceConfiguration.ContentApplicationId + "-legacy" : _providerApprenticeshipsServiceConfiguration.ContentApplicationId;
        var cacheKey = $"{applicationId}_{request.ContentType}".ToLowerInvariant();

        try
        {     
            var contentBanner = await _contentApiClient.Get(request.ContentType, applicationId);
            return new GetClientContentResponse
            {
                Content = contentBanner
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Content for {CacheKey}", cacheKey);

            return new GetClientContentResponse
            {
                HasFailed = true
            };
        }
    }
}