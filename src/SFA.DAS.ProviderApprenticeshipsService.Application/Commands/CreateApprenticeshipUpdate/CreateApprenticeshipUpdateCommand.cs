﻿using FluentValidation.Attributes;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeshipUpdate
{
    [Validator(typeof(CreateApprenticeshipUpdateCommandValidator))]
    public class CreateApprenticeshipUpdateCommand : IRequest
    {
        public string UserId { get; set; }
        public long ProviderId { get; set; }
        public ApprenticeshipUpdate ApprenticeshipUpdate { get; set; }
        public string UserEmailAddress { get; set; }
        public string UserDisplayName { get; set; }
    }
}
