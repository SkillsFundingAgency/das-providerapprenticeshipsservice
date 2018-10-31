using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetPendingApprenticeshipUpdate
{
    public class GetPendingApprenticeshipUpdateQueryHandler: IRequestHandler<GetPendingApprenticeshipUpdateQueryRequest,GetPendingApprenticeshipUpdateQueryResponse>
    {
        private readonly IProviderCommitmentsApi _apiClient;
        
        public GetPendingApprenticeshipUpdateQueryHandler(IProviderCommitmentsApi apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<GetPendingApprenticeshipUpdateQueryResponse> Handle(GetPendingApprenticeshipUpdateQueryRequest message, CancellationToken cancellationToken)
        {
            var result = await _apiClient.GetPendingApprenticeshipUpdate(message.ProviderId, message.ApprenticeshipId);

            return new GetPendingApprenticeshipUpdateQueryResponse
            {
                ApprenticeshipUpdate = result
            };
        }
    }
}
