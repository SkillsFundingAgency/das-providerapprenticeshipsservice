using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLock
{
    public sealed class GetApprenticeshipDataLockRequest : IAsyncRequest<GetApprenticeshipDataLockResponse>
    {
        public long ApprenticeshipId { get; set; }
    }
}