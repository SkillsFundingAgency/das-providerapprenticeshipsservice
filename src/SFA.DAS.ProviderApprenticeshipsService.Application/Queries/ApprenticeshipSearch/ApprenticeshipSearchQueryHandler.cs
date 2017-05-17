using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.ApprenticeshipSearch
{
    public class ApprenticeshipSearchQueryHandler : IAsyncRequestHandler<ApprenticeshipSearchQueryRequest, ApprenticeshipSearchQueryResponse>
    {

        private readonly IProviderCommitmentsApi _commitmentsApi;

        public ApprenticeshipSearchQueryHandler(IProviderCommitmentsApi providerCommitmentsApi)
        {
            if (providerCommitmentsApi == null)
                throw new ArgumentNullException(nameof(providerCommitmentsApi));
            _commitmentsApi = providerCommitmentsApi;
        }

        public async Task<ApprenticeshipSearchQueryResponse> Handle(ApprenticeshipSearchQueryRequest message)
        {
            var data = await _commitmentsApi.GetProviderApprenticeships(message.ProviderId, message.Query);

            return new ApprenticeshipSearchQueryResponse
            {
                Apprenticeships = data.Apprenticeships.ToList(),
                Facets = data.Facets
            };
        }
    }
}
