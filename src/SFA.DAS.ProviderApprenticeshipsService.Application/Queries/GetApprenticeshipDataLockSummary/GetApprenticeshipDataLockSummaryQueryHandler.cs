using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLockSummary
{
    public class GetApprenticeshipDataLockSummaryQueryHandler : IAsyncRequestHandler<GetApprenticeshipDataLockSummaryQueryRequest,GetApprenticeshipDataLockSummaryQueryResponse>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;

        public GetApprenticeshipDataLockSummaryQueryHandler(IProviderCommitmentsApi commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
        }

        public async Task<GetApprenticeshipDataLockSummaryQueryResponse> Handle(GetApprenticeshipDataLockSummaryQueryRequest command)
        {
            var response = await _commitmentsApi.GetDataLockSummary(command.ProviderId, command.ApprenticeshipId);

            return new GetApprenticeshipDataLockSummaryQueryResponse
            {
                DataLockSummary = response
            };
        }
    }
}
