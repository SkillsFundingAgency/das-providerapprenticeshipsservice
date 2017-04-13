using FluentValidation.Attributes;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeshipUpdate
{
    [Validator(typeof(CreateApprenticeshipUpdateCommandValidator))]
    public class CreateApprenticeshipUpdateCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public long ProviderId { get; set; }
        public ApprenticeshipUpdate ApprenticeshipUpdate { get; set; }
    }
}
