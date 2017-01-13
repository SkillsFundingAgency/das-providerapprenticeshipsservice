using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships
{
    public sealed class BulkUploadApprenticeshipsCommandHandler : AsyncRequestHandler<BulkUploadApprenticeshipsCommand>
    {
        private ICommitmentsApi _commitmentsApi;

        public BulkUploadApprenticeshipsCommandHandler(ICommitmentsApi commitmentsApi)
        {
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));

            _commitmentsApi = commitmentsApi;
        }

        protected async override Task HandleCore(BulkUploadApprenticeshipsCommand message)
        {
            var validationResult = new BulkUploadApprenticeshipsCommandValidator().Validate(message);

            if (!validationResult.IsValid)
                throw new InvalidRequestException(validationResult.Errors);

            await _commitmentsApi.BulkUploadApprenticeships(message.ProviderId, message.CommitmentId, message.Apprenticeships);
        }
    }
}
