using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.ApprenticeshipSearch
{
    public class ApprenticeshipSearchQueryHandler : IRequestHandler<ApprenticeshipSearchQueryRequest, ApprenticeshipSearchQueryResponse>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly IProviderCommitmentsLogger _logger;

        public ApprenticeshipSearchQueryHandler(IProviderCommitmentsApi providerCommitmentsApi, IProviderCommitmentsLogger logger)
        {
            _commitmentsApi = providerCommitmentsApi;
            _logger = logger;
        }

        public async Task<ApprenticeshipSearchQueryResponse> Handle(ApprenticeshipSearchQueryRequest message, CancellationToken cancellationToken)
        {
            _logger.Trace($"Performing apprenticeship search against api for provider {message.ProviderId}",
                message.ProviderId);

            var data = await _commitmentsApi.GetProviderApprenticeships(message.ProviderId, message.Query);

            return new ApprenticeshipSearchQueryResponse
            {
                Apprenticeships = data.Apprenticeships.ToList(),
                SearchKeyword = data.SearchKeyword,
                Facets = data.Facets,
                TotalApprenticeships = data.TotalApprenticeships,
                TotalApprenticeshipsBeforeFilter = data.TotalApprenticeshipsBeforeFilter,
                PageNumber = data.PageNumber,
                TotalPages = data.TotalPages,
                PageSize = data.PageSize
            };
        }
    }
}
