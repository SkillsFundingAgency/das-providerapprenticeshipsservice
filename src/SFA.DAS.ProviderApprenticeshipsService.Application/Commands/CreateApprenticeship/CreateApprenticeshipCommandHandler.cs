using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeship
{
    public class CreateApprenticeshipCommandHandler : AsyncRequestHandler<CreateApprenticeshipCommand>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly IValidator<CreateApprenticeshipCommand> _validator;

        public CreateApprenticeshipCommandHandler(
            IProviderCommitmentsApi commitmentsApi,
            IValidator<CreateApprenticeshipCommand> validator)
        {
            _commitmentsApi = commitmentsApi;
            _validator = validator;
        }

        protected override async Task HandleCore(CreateApprenticeshipCommand message)
        {
            var validationResult = _validator.Validate(message);
            if (!validationResult.IsValid)
                throw new InvalidRequestException(validationResult.Errors);

            var request = new ApprenticeshipRequest
            {
                UserId = message.UserId,
                Apprenticeship = message.Apprenticeship,
                LastUpdatedByInfo = new LastUpdateInfo
                {
                    EmailAddress = message.UserEmailAddress,
                    Name = message.UserDisplayName
                }
            };

            await _commitmentsApi.CreateProviderApprenticeship(message.ProviderId, message.Apprenticeship.CommitmentId, request);
        }
    }
}