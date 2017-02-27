using System;
using System.Threading.Tasks;

using FluentValidation;
using MediatR;

using SFA.DAS.Commitments.Api.Client;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteCommitment
{
    public sealed class DeleteCommitmentCommandHandler : AsyncRequestHandler<DeleteCommitmentCommand>
    {
        private readonly AbstractValidator<DeleteCommitmentCommand> _validator;

        private readonly ICommitmentsApi _commitmentsApi;

        public DeleteCommitmentCommandHandler(AbstractValidator<DeleteCommitmentCommand> validator, ICommitmentsApi commitmentsApi)
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

            // TODO: LWA - need to pass in UserId
            return _commitmentsApi.DeleteProviderCommitment(message.ProviderId, message.CommitmentId, string.Empty);
        }
    }
}