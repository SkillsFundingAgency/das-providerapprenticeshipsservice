using System.Threading.Tasks;

using FluentValidation;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SaveBulkUploadFile
{
    public class SaveBulkUploadFileHandler : IAsyncRequestHandler<SaveBulkUploadFileCommand, long>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly AbstractValidator<SaveBulkUploadFileCommand> _validator;

        public SaveBulkUploadFileHandler(
            IProviderCommitmentsApi commitmentsApi,
            AbstractValidator<SaveBulkUploadFileCommand> validator)
        {
            _commitmentsApi = commitmentsApi;
            _validator = validator;
        }

        public async Task<long> Handle(SaveBulkUploadFileCommand message)
        {
            var validationResult = _validator.Validate(message);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

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