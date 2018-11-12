using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SaveBulkUploadFile
{
    public class SaveBulkUploadFileHandler : IRequestHandler<SaveBulkUploadFileCommand, long>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly IValidator<SaveBulkUploadFileCommand> _validator;

        public SaveBulkUploadFileHandler(
            IProviderCommitmentsApi commitmentsApi,
            IValidator<SaveBulkUploadFileCommand> validator)
        {
            _commitmentsApi = commitmentsApi;
            _validator = validator;
        }

        public async Task<long> Handle(SaveBulkUploadFileCommand message, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(message);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var request = new BulkUploadFileRequest
            {
              CommitmentId = message.CommitmentId,
              Data = message.FileContent,
              FileName = message.FileName
            };

            return await _commitmentsApi.BulkUploadFile(message.ProviderId, request);
        }
    }
}