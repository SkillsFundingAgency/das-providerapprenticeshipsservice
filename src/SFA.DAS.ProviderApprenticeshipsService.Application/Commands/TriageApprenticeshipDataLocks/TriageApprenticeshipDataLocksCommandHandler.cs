using System;
using System.Threading;
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
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly IValidator<TriageApprenticeshipDataLocksCommand> _validator;
        private readonly ILog _logger;

        public TriageApprenticeshipDataLocksCommandHandler(
            IProviderCommitmentsApi commitmentsApi, 
            ILog logger, 
            IValidator<TriageApprenticeshipDataLocksCommand> validator)
        {
            _commitmentsApi = commitmentsApi;
            _logger = logger;
            _validator = validator;
        }

        protected override async Task Handle(TriageApprenticeshipDataLocksCommand command, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(command);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var triageSubmission = new DataLockTriageSubmission
            {
                TriageStatus = command.TriageStatus,
                UserId = command.UserId
            };

            try
            {
                await _commitmentsApi.PatchDataLocks(command.ProviderId, command.ApprenticeshipId, triageSubmission);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error calling commitment API when updating multiple price data locks to status {command.TriageStatus} for apprenticeship {command.ApprenticeshipId}");
                throw;
            }
        }
    }
}
