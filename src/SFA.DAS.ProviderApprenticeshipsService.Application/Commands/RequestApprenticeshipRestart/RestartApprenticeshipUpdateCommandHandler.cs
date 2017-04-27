using System;
using System.Threading.Tasks;

using FluentValidation;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.RequestApprenticeshipRestart
{
    public sealed class RestartApprenticeshipUpdateCommandHandler : AsyncRequestHandler<RestartApprenticeshipCommand>
    {
        private readonly AbstractValidator<RestartApprenticeshipCommand> _validator;

        private readonly IDataLockApi _dataLockApi;

        private IProviderCommitmentsApi _commitmentsApi;

        public RestartApprenticeshipUpdateCommandHandler(
            AbstractValidator<RestartApprenticeshipCommand> validator,
            IDataLockApi dataLockApi)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (dataLockApi == null)
                throw new ArgumentNullException(nameof(dataLockApi));

            _validator = validator;
            _dataLockApi = dataLockApi;
        }

        protected override async Task HandleCore(RestartApprenticeshipCommand command)
        {
            _validator.ValidateAndThrow(command);

            var dataLock = await _dataLockApi.GetDataLock(command.ApprenticeshipId, command.ProviderId);

            dataLock.TriageStatus = TriageStatus.Restart;
            await _dataLockApi.PatchDataLock(command.ApprenticeshipId, dataLock);

            //var submission = new RequestApprenticeshipRestartSubmission
            //{
            //    UpdateStatus = TriageStatus.RestartApprenticeship
            //};

            //await _commitmentsApi.PatchDataLock(command.ProviderId, command.ApprenticeshipId, submission);
        }
    }
}