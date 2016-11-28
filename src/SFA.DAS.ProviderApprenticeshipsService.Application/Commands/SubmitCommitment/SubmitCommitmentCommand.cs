using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment
{
    using SFA.DAS.Commitments.Api.Types;

    public class SubmitCommitmentCommand : IAsyncRequest
    {
        public long ProviderId { get; set; }

        public long CommitmentId { get; set; }

        public string Message { get; set; }

        public AgreementStatus AgreementStatus { get; set; }

        public bool CreateTask { get; set; }
    }
}