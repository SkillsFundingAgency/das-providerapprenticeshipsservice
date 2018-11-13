using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships
{
    public sealed class BulkUploadApprenticeshipsCommandHandler : AsyncRequestHandler<BulkUploadApprenticeshipsCommand>
    {
        private readonly IProviderCommitmentsApi _providerCommitmentsApi;
        private readonly IProviderCommitmentsLogger _logger;

        public BulkUploadApprenticeshipsCommandHandler(IProviderCommitmentsApi providerCommitmentsApi, IProviderCommitmentsLogger logger)
        {
            _providerCommitmentsApi  = providerCommitmentsApi;
            _logger = logger;
        }

        protected override async Task Handle(BulkUploadApprenticeshipsCommand message, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var validationResult = new BulkUploadApprenticeshipsCommandValidator().Validate(message);

            if (!validationResult.IsValid)
                throw new InvalidRequestException(validationResult.Errors);

            var request = new BulkApprenticeshipRequest
            {
                UserId = message.UserId,
                Apprenticeships = message.Apprenticeships,
                LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = message.UserEmailAddress, Name = message.UserDisplayName }
            };

            await _providerCommitmentsApi.BulkUploadApprenticeships(message.ProviderId, message.CommitmentId, request);

            _logger.Trace($"Took {stopwatch.ElapsedMilliseconds} milliseconds to Bulk Upload {message.Apprenticeships.Count} apprentices to Commitments Api");
        }
    }
}
