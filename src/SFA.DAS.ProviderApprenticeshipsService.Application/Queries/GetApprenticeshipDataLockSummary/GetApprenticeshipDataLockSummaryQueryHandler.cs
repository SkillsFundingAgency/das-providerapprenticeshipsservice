using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLockSummary
{
    public class GetApprenticeshipDataLockSummaryQueryHandler : IRequestHandler<GetApprenticeshipDataLockSummaryQueryRequest,GetApprenticeshipDataLockSummaryQueryResponse>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;

        public GetApprenticeshipDataLockSummaryQueryHandler(IProviderCommitmentsApi commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
        }

        public async Task<GetApprenticeshipDataLockSummaryQueryResponse> Handle(GetApprenticeshipDataLockSummaryQueryRequest command, CancellationToken cancellationToken)
        {
            var response = await _commitmentsApi.GetDataLockSummary(command.ProviderId, command.ApprenticeshipId);

            return new GetApprenticeshipDataLockSummaryQueryResponse
            {
                DataLockSummary = response
            };
        }
    }
}
