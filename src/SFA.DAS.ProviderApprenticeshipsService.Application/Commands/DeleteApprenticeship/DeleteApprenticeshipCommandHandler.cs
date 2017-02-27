using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Client;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteApprenticeship
{
    public sealed class DeleteApprenticeshipCommandHandler : AsyncRequestHandler<DeleteApprenticeshipCommand>
    {
        private readonly ICommitmentsApi _commitmentsApi;
        private readonly AbstractValidator<DeleteApprenticeshipCommand> _validator;

        public DeleteApprenticeshipCommandHandler(AbstractValidator<DeleteApprenticeshipCommand> validator, ICommitmentsApi commitmentsApi)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));

            _validator = validator;
            _commitmentsApi = commitmentsApi;
        }

        protected override Task HandleCore(DeleteApprenticeshipCommand message)
        {
            _validator.ValidateAndThrow(message);

            // TODO: LWA - Need to pass in UserID
            return _commitmentsApi.DeleteProviderApprenticeship(message.ProviderId, message.ApprenticeshipId, string.Empty);
        }
    }
}
