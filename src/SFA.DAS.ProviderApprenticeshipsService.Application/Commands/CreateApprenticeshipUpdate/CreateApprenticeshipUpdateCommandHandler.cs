using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeshipUpdate
{
    public class CreateApprenticeshipUpdateCommandHandler: AsyncRequestHandler<CreateApprenticeshipUpdateCommand>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly AbstractValidator<CreateApprenticeshipUpdateCommand> _validator;

        public CreateApprenticeshipUpdateCommandHandler(IProviderCommitmentsApi commitmentsApi, AbstractValidator<CreateApprenticeshipUpdateCommand> validator)
        {
            if(commitmentsApi==null)
                throw new ArgumentNullException(nameof(commitmentsApi));
            if(validator==null)
                throw new ArgumentNullException(nameof(validator));

            _commitmentsApi = commitmentsApi;
            _validator = validator;
        }

        protected override async Task HandleCore(CreateApprenticeshipUpdateCommand command)
        {
            var validationResult = _validator.Validate(command);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var request = new ApprenticeshipUpdateRequest
            {
                ApprenticeshipUpdate = command.ApprenticeshipUpdate,
                UserId = command.UserId
            };

            await _commitmentsApi.CreateApprenticeshipUpdate(command.ProviderId, command.ApprenticeshipUpdate.ApprenticeshipId, request);
        }
    }
}
