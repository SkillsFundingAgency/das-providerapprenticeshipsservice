using System;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteCommitment
{
    public sealed class DeleteCommitmentCommandHandler : AsyncRequestHandler<DeleteCommitmentCommand>
    {
        private readonly IValidator<DeleteCommitmentCommand> _validator;
        private readonly IProviderCommitmentsApi _commitmentsApi;

        public DeleteCommitmentCommandHandler(IValidator<DeleteCommitmentCommand> validator, IProviderCommitmentsApi commitmentsApi)
        {
            _validator = validator;
            _commitmentsApi = commitmentsApi;
        }

        protected override Task Handle(DeleteCommitmentCommand message, CancellationToken cancellationToken)
        {
            _validator.ValidateAndThrow(message);

            return _commitmentsApi.DeleteProviderCommitment(message.ProviderId, message.CommitmentId,
                new DeleteRequest { UserId = message.UserId, LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = message.UserEmailAddress, Name = message.UserDisplayName } });
        }
    }
}