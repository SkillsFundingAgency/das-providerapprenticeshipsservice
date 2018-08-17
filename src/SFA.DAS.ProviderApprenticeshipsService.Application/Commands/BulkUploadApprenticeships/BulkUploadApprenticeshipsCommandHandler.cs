using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships
{
    public sealed class BulkUploadApprenticeshipsCommandHandler : AsyncRequestHandler<BulkUploadApprenticeshipsCommand>
    {
        private readonly IProviderCommitmentsApi _providerCommitmentsApi;

        public BulkUploadApprenticeshipsCommandHandler(IProviderCommitmentsApi providerCommitmentsApi)
        {
            _providerCommitmentsApi  = providerCommitmentsApi;
        }

        protected override async Task HandleCore(BulkUploadApprenticeshipsCommand message)
        {
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
        }
    }
}
