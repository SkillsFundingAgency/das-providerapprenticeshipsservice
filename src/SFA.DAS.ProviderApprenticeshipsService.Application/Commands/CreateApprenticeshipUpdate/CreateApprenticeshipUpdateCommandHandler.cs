using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeshipUpdate
{
    public class CreateApprenticeshipUpdateCommandHandler: AsyncRequestHandler<CreateApprenticeshipUpdateCommand>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly IValidator<CreateApprenticeshipUpdateCommand> _validator;

        public CreateApprenticeshipUpdateCommandHandler(
            IProviderCommitmentsApi commitmentsApi,
            IValidator<CreateApprenticeshipUpdateCommand> validator)
        {
            _commitmentsApi = commitmentsApi;
            _validator = validator;
        }

        protected override Task Handle(CreateApprenticeshipUpdateCommand command, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(command);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var request = new ApprenticeshipUpdateRequest
            {
                ApprenticeshipUpdate = command.ApprenticeshipUpdate,
                UserId = command.UserId,
                LastUpdatedByInfo = new LastUpdateInfo
                {
                    EmailAddress = command.UserEmailAddress,
                    Name = command.UserDisplayName
                }
            };

            return _commitmentsApi.CreateApprenticeshipUpdate(command.ProviderId, command.ApprenticeshipUpdate.ApprenticeshipId, request);
        }
    }
}
