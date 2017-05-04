using System;
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
        private readonly AbstractValidator<DeleteCommitmentCommand> _validator;

        private readonly IProviderCommitmentsApi _commitmentsApi;

        public DeleteCommitmentCommandHandler(AbstractValidator<DeleteCommitmentCommand> validator, IProviderCommitmentsApi commitmentsApi)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));

            _validator = validator;
            _commitmentsApi = commitmentsApi;
        }

        protected override Task HandleCore(DeleteCommitmentCommand message)
        {
            _validator.ValidateAndThrow(message);

            return _commitmentsApi.DeleteProviderCommitment(message.ProviderId, message.CommitmentId,
                new DeleteRequest { UserId = message.UserId, LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = message.UserEmailAddress, Name = message.UserDisplayName } });
        }
    }
}