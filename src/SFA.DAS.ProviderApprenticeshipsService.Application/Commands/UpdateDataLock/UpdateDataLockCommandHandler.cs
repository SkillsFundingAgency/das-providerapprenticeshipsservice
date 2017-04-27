using System;
using System.Threading.Tasks;

using FluentValidation;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateDataLock
{
    public sealed class UpdateDataLockCommandHandler : AsyncRequestHandler<UpdateDataLockCommand>
    {
        private readonly AbstractValidator<UpdateDataLockCommand> _validator;

        private readonly IDataLockApi _dataLockApi;

        private readonly ILog _logger;

        public UpdateDataLockCommandHandler(
            AbstractValidator<UpdateDataLockCommand> validator,
            IDataLockApi dataLockApi,
            ILog logger)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (dataLockApi == null)
                throw new ArgumentNullException(nameof(dataLockApi));
            if (dataLockApi == null)
                throw new ArgumentNullException(nameof(logger));

            _validator = validator;
            _dataLockApi = dataLockApi;
            _logger = logger;
        }

        protected override async Task HandleCore(UpdateDataLockCommand command)
        {
            _validator.ValidateAndThrow(command);
            try
            {
                var dataLock = await _dataLockApi.GetDataLock(command.ApprenticeshipId, command.DataLockEventId);
                dataLock.TriageStatus = command.TriageStatus;
                await _dataLockApi.PatchDataLock(command.ApprenticeshipId, dataLock);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error calling commitment API when updating data lock to status {command.TriageStatus} for apprenticeship {command.ApprenticeshipId} and data lock with ID: {command.DataLockEventId}");
            }
        }
    }
}