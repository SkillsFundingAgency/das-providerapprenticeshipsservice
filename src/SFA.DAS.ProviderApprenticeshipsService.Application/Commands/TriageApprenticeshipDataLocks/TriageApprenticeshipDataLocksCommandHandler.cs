using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.TriageApprenticeshipDataLocks
{
    public class TriageApprenticeshipDataLocksCommandHandler : AsyncRequestHandler<TriageApprenticeshipDataLocksCommand>
    {
        private readonly IDataLockApi _dataLockApi;
        private readonly AbstractValidator<TriageApprenticeshipDataLocksCommand> _validator;
        private readonly ILog _logger;

        public TriageApprenticeshipDataLocksCommandHandler(IDataLockApi dataLockApi, ILog logger, AbstractValidator<TriageApprenticeshipDataLocksCommand> validator)
        {
            _dataLockApi = dataLockApi;
            _logger = logger;
            _validator = validator;
        }

        protected override async Task HandleCore(TriageApprenticeshipDataLocksCommand command)
        {
            var validationResult = _validator.Validate(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var triageSubmission = new DataLockTriageSubmission
            {
                TriageStatus = command.TriageStatus,
                UserId = command.UserId
            };

            try
            {
                await _dataLockApi.PatchDataLocks(command.ApprenticeshipId, triageSubmission);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error calling commitment API when updating multiple price data locks to status {command.TriageStatus} for apprenticeship {command.ApprenticeshipId}");
                throw;
            }
        }
    }
}
