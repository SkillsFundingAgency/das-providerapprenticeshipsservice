using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateApprenticeship
{
    public class UpdateApprenticeshipCommandHandler : AsyncRequestHandler<UpdateApprenticeshipCommand>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly IValidator<UpdateApprenticeshipCommand> _validator;

        public UpdateApprenticeshipCommandHandler(
            IValidator<UpdateApprenticeshipCommand> validator,
            IProviderCommitmentsApi commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
            _validator = validator;
        }

        protected override Task Handle(UpdateApprenticeshipCommand message, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(message);
            if (!validationResult.IsValid)
                throw new InvalidRequestException(validationResult.Errors);

            var request = new ApprenticeshipRequest
            {
                UserId = message.UserId,
                Apprenticeship = message.Apprenticeship,
                LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = message.UserEmailAddress, Name = message.UserDisplayName }
            };

            return _commitmentsApi.UpdateProviderApprenticeship(message.ProviderId, message.Apprenticeship.CommitmentId, message.Apprenticeship.Id, request);
        }
    }
}