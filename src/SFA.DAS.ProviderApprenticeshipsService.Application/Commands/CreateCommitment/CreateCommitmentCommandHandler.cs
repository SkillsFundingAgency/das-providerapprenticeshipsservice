using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateCommitment
{
    public class CreateCommitmentCommandHandler : IRequestHandler<CreateCommitmentCommand, CreateCommitmentCommandResponse>
    {
        private IProviderCommitmentsApi _commitmentsApi;
        private readonly IValidator<CreateCommitmentCommand> _validator;

        public CreateCommitmentCommandHandler(IProviderCommitmentsApi commitmentsApi, IValidator<CreateCommitmentCommand> validator)
        {
            _commitmentsApi = commitmentsApi;
            _validator = validator;
        }

        public async Task<CreateCommitmentCommandResponse> Handle(CreateCommitmentCommand request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var result = await _commitmentsApi.CreateProviderCommitment(request.Commitment.ProviderId.Value, new CommitmentRequest
            {
                Commitment = request.Commitment,
                LastAction = LastAction.None,
                Message = request.Message,
                UserId = request.UserId
            });

            return new CreateCommitmentCommandResponse
            {
                CommitmentId = result.Id
            };
        }
    }
}
