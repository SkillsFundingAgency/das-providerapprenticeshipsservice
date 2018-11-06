using MediatR;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateCommitment
{
    public class CreateCommitmentCommand : IRequest<CreateCommitmentCommandResponse>
    {
        public Commitment Commitment { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
    }
}
