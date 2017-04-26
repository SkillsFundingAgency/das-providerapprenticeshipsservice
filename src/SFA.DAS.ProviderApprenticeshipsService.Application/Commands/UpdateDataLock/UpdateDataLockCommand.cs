using MediatR;

using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateDataLock
{
    public sealed class UpdateDataLockCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public long ProviderId { get; set; }
        public long ApprenticeshipId { get; set; }
        public TriageStatus TriageStatus { get; set; }
    }
}