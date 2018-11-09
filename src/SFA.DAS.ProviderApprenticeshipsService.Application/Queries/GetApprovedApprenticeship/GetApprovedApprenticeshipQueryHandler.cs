using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprovedApprenticeship
{
    public class GetApprovedApprenticeshipQueryHandler : IAsyncRequestHandler<GetApprovedApprenticeshipQueryRequest, GetApprovedApprenticeshipQueryResponse>
    {
        private readonly IProviderCommitmentsApi _providerCommitmentsApi;

        public GetApprovedApprenticeshipQueryHandler(IProviderCommitmentsApi providerCommitmentsApi)
        {
            _providerCommitmentsApi = providerCommitmentsApi;
        }

        public async Task<GetApprovedApprenticeshipQueryResponse> Handle(GetApprovedApprenticeshipQueryRequest message)
        {
            var result = await _providerCommitmentsApi.GetApprovedApprenticeship(message.ProviderId, message.ApprovedApprenticeshipId);
            return new GetApprovedApprenticeshipQueryResponse
            {
                ApprovedApprenticeship = result
            };
        }
    }
}
