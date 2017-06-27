using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLocks
{
    public sealed class GetApprenticeshipDataLocksRequest : IAsyncRequest<GetApprenticeshipDataLocksResponse>
    {
        public long ApprenticeshipId { get; set; }
    }
}