using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships
{
    public sealed class BulkUploadApprenticeshipsCommandHandler : AsyncRequestHandler<BulkUploadApprenticeshipsCommand>
    {
        private IProviderCommitmentsApi _providerCommitmentsApi;

        public BulkUploadApprenticeshipsCommandHandler(IProviderCommitmentsApi providerCommitmentsApi)
        {
            if (providerCommitmentsApi  == null)
                throw new ArgumentNullException(nameof(providerCommitmentsApi));

            _providerCommitmentsApi  = providerCommitmentsApi;
        }

        protected async override Task HandleCore(BulkUploadApprenticeshipsCommand message)
        {
            var validationResult = new BulkUploadApprenticeshipsCommandValidator().Validate(message);

            if (!validationResult.IsValid)
                throw new InvalidRequestException(validationResult.Errors);

            var request = new BulkApprenticeshipRequest
            {
                UserId = message.UserId,
                Apprenticeships = message.Apprenticeships
            };

            await _providerCommitmentsApi.BulkUploadApprenticeships(message.ProviderId, message.CommitmentId, request);
        }
    }
}
