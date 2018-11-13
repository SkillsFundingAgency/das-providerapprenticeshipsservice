using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.ReviewApprenticeshipUpdate
{
    public sealed class ReviewApprenticeshipUpdateCommandHandler : AsyncRequestHandler<ReviewApprenticeshipUpdateCommand>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly IValidator<ReviewApprenticeshipUpdateCommand> _validator;

        public ReviewApprenticeshipUpdateCommandHandler(IValidator<ReviewApprenticeshipUpdateCommand> validator, IProviderCommitmentsApi commitmentsApi)
        {
            _validator = validator;
            _commitmentsApi = commitmentsApi;
        }

        protected override Task Handle(ReviewApprenticeshipUpdateCommand command, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(command);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var submission = new ApprenticeshipUpdateSubmission
            {
                UpdateStatus = command.IsApproved ? ApprenticeshipUpdateStatus.Approved : ApprenticeshipUpdateStatus.Rejected,
                UserId = command.UserId,
                LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = command.UserEmailAddress, Name = command.UserDisplayName }
            };

            return _commitmentsApi.PatchApprenticeshipUpdate(command.ProviderId, command.ApprenticeshipId, submission);
        }
    }
}
