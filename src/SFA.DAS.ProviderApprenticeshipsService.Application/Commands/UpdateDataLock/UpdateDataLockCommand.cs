using MediatR;

using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateDataLock
{
    public sealed class UpdateDataLockCommand : IAsyncRequest
    {
        public long ApprenticeshipId { get; set; }
        public TriageStatus TriageStatus { get; set; }
        public string UserId { get; set; }
        public long ProviderId { get; set; }
    }
}