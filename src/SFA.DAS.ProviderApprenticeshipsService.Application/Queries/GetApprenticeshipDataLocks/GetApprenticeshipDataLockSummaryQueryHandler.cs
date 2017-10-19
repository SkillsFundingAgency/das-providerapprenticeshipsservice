using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLocks
{
    public class GetApprenticeshipDataLocksQueryHandler : IAsyncRequestHandler<GetApprenticeshipDataLocksQueryRequest,GetApprenticeshipDataLocksQueryResponse>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;

        public GetApprenticeshipDataLocksQueryHandler(IProviderCommitmentsApi commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
        }

        public async Task<GetApprenticeshipDataLocksQueryResponse> Handle(GetApprenticeshipDataLocksQueryRequest command)
        {
            var response = await _commitmentsApi.GetDataLocks(command.ProviderId, command.ApprenticeshipId);

            return new GetApprenticeshipDataLocksQueryResponse
            {
                DataLockSummary = response
            };
        }
    }
}
