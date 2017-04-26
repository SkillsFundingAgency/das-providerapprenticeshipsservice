using System;
using System.Threading.Tasks;

using FluentValidation;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.RequestApprenticeshipRestart
{
    public sealed class RestartApprenticeshipUpdateCommandHandler : AsyncRequestHandler<RestartApprenticeshipCommand>
    {
        private readonly AbstractValidator<RestartApprenticeshipCommand> _validator;

        private IProviderCommitmentsApi _commitmentsApi;

        public RestartApprenticeshipUpdateCommandHandler(
            AbstractValidator<RestartApprenticeshipCommand> validator,
            IProviderCommitmentsApi commitmentsApi)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));

            _validator = validator;
            _commitmentsApi = commitmentsApi;
        }

        protected override async Task HandleCore(RestartApprenticeshipCommand command)
        {
            _validator.ValidateAndThrow(command);

            //var submission = new RequestApprenticeshipRestartSubmission
            //{
            //    UpdateStatus = TriageStatus.RestartApprenticeship
            //};

            //await _commitmentsApi.PatchDataLock(command.ProviderId, command.ApprenticeshipId, submission);
        }
    }
}