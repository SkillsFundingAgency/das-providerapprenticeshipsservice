using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLocks
{
    public class GetApprenticeshipDataLocksQueryRequest : IAsyncRequest<GetApprenticeshipDataLocksQueryResponse>
    {
        public long ApprenticeshipId { get; set; }

        public long ProviderId { get; set; }
    }
}
