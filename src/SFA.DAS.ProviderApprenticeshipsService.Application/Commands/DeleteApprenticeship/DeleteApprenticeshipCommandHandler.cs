using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteApprenticeship
{
    public sealed class DeleteApprenticeshipCommandHandler : AsyncRequestHandler<DeleteApprenticeshipCommand>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly AbstractValidator<DeleteApprenticeshipCommand> _validator;

        public DeleteApprenticeshipCommandHandler(AbstractValidator<DeleteApprenticeshipCommand> validator, IProviderCommitmentsApi commitmentsApi)
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

            return _commitmentsApi.DeleteProviderApprenticeship(message.ProviderId, message.ApprenticeshipId, new DeleteRequest { UserId = message.UserId });
        }
    }
}
