using MediatR;

using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateDataLock
{
    public sealed class UpdateDataLockCommand : IAsyncRequest
    {
        public long DataLockEventId { get; set; }
        public long ApprenticeshipId { get; set; }
        public TriageStatus TriageStatus { get; set; }
    }
}