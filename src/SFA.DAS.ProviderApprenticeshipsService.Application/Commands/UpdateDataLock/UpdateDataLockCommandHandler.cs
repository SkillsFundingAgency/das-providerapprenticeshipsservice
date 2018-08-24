using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateDataLock
{
    public sealed class UpdateDataLockCommandHandler : AsyncRequestHandler<UpdateDataLockCommand>
    {
        private readonly IValidator<UpdateDataLockCommand> _validator;
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly ILog _logger;

        public UpdateDataLockCommandHandler(
            IValidator<UpdateDataLockCommand> validator,
            IProviderCommitmentsApi commitmentsApi,
            ILog logger)
        {
            _validator = validator;
            _commitmentsApi = commitmentsApi;
            _logger = logger;
        }

        protected override async Task HandleCore(UpdateDataLockCommand command)
        {
            _validator.ValidateAndThrow(command);
            try
            {
                var submission = new DataLockTriageSubmission
                        {
                            TriageStatus = command.TriageStatus,
                            UserId = command.UserId
                        };

                await _commitmentsApi.PatchDataLocks(command.ProviderId, command.ApprenticeshipId, submission);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error calling commitment API when updating data lock to status {command.TriageStatus} for apprenticeship {command.ApprenticeshipId}");
                throw;
            }
        }
    }
}