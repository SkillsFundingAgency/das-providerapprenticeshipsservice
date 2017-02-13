using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteCommitment
{
    public class DeleteCommitmentCommand : IAsyncRequest
    {
        public long ProviderId { get; set; }

        public long CommitmentId { get; set; }
    }
}
