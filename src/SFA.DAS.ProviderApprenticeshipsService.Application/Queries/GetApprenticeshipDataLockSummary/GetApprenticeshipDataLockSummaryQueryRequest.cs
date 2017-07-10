using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLockSummary
{
    public class GetApprenticeshipDataLockSummaryQueryRequest : IAsyncRequest<GetApprenticeshipDataLockSummaryQueryResponse>
    {
        public long ApprenticeshipId { get; set; }

        public long ProviderId { get; set; }
    }
}
