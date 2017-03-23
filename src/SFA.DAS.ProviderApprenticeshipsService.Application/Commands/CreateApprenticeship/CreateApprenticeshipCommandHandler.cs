﻿using System;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeship
{
    public class CreateApprenticeshipCommandHandler : AsyncRequestHandler<CreateApprenticeshipCommand>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly CreateApprenticeshipCommandValidator _validator;

        public CreateApprenticeshipCommandHandler(IProviderCommitmentsApi commitmentsApi)
        {
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));
            _commitmentsApi = commitmentsApi;
            _validator = new CreateApprenticeshipCommandValidator();
        }

        protected override async Task HandleCore(CreateApprenticeshipCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid)
                throw new InvalidRequestException(validationResult.Errors);

            var request = new ApprenticeshipRequest
            {
                UserId = message.UserId,
                Apprenticeship = message.Apprenticeship
            };

            await _commitmentsApi.CreateProviderApprenticeship(message.ProviderId, message.Apprenticeship.CommitmentId, request);
        }
    }
}