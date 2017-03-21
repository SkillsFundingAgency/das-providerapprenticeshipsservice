using System;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;


namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateApprenticeship
{
    public class UpdateApprenticeshipCommandHandler : AsyncRequestHandler<UpdateApprenticeshipCommand>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly UpdateApprenticeshipCommandValidator _validator;

        public UpdateApprenticeshipCommandHandler(IProviderCommitmentsApi commitmentsApi)
        {
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));
            _commitmentsApi = commitmentsApi;
            _validator = new UpdateApprenticeshipCommandValidator();
        }

        protected override async Task HandleCore(UpdateApprenticeshipCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid)
                throw new InvalidRequestException(validationResult.Errors);

            var request = new ApprenticeshipRequest
            {
                UserId = message.UserId,
                Apprenticeship = message.Apprenticeship
            };

            await _commitmentsApi.UpdateProviderApprenticeship(message.ProviderId, message.Apprenticeship.CommitmentId, message.Apprenticeship.Id, request);
        }
    }
}