using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UndoApprenticeshipUpdate
{
    public sealed class UndoApprenticeshipUpdateCommandHandler : AsyncRequestHandler<UndoApprenticeshipUpdateCommand>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly IValidator<UndoApprenticeshipUpdateCommand> _validator;

        public UndoApprenticeshipUpdateCommandHandler(IValidator<UndoApprenticeshipUpdateCommand> validator, IProviderCommitmentsApi commitmentsApi)
        {
            _validator = validator;
            _commitmentsApi = commitmentsApi;
        }

        protected override async Task HandleCore(UndoApprenticeshipUpdateCommand command)
        {
            var validationResult = _validator.Validate(command);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var submission = new ApprenticeshipUpdateSubmission
            {
                UpdateStatus = ApprenticeshipUpdateStatus.Deleted,
                UserId = command.UserId,
                LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = command.UserEmailAddress, Name = command.UserDisplayName }
            };

            await _commitmentsApi.PatchApprenticeshipUpdate(command.ProviderId, command.ApprenticeshipId, submission);
        }
    }
}
