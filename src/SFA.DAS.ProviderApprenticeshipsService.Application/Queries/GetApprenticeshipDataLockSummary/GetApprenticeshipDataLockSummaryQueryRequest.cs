using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLockSummary
{
    public class GetApprenticeshipDataLockSummaryQueryRequest : IRequest<GetApprenticeshipDataLockSummaryQueryResponse>
    {
        public long ApprenticeshipId { get; set; }

        public long ProviderId { get; set; }
    }
}
