using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetPendingApprenticeshipUpdate
{
    public class GetPendingApprenticeshipUpdateQueryHandler: IAsyncRequestHandler<GetPendingApprenticeshipUpdateQueryRequest,GetPendingApprenticeshipUpdateQueryResponse>
    {
        private readonly IProviderCommitmentsApi _apiClient;
        
        public GetPendingApprenticeshipUpdateQueryHandler(IProviderCommitmentsApi apiClient)
        {
            if(apiClient==null)
                throw new ArgumentNullException(nameof(apiClient));

            _apiClient = apiClient;
        }

        public async Task<GetPendingApprenticeshipUpdateQueryResponse> Handle(GetPendingApprenticeshipUpdateQueryRequest message)
        {
            var result = await _apiClient.GetPendingApprenticeshipUpdate(message.ProviderId, message.ApprenticeshipId);

            return new GetPendingApprenticeshipUpdateQueryResponse
            {
                ApprenticeshipUpdate = result
            };
        }
    }
}
