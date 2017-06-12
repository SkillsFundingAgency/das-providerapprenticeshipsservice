using MediatR;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.TriageApprenticeshipDataLocks
{
    public class TriageApprenticeshipDataLocksCommand : IAsyncRequest
    {
        public long ApprenticeshipId { get; set; }
        public TriageStatus TriageStatus { get; set; }
        public string UserId { get; set; }
    }
}
