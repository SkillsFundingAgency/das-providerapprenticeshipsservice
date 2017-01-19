using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload
{
    public sealed class BulkUploader
    {
        private readonly IMediator _mediator;
        private readonly IBulkUploadValidator _bulkUploadValidator;
        private readonly IProviderCommitmentsLogger _logger;
        private readonly IBulkUploadFileParser _fileParser;

        public BulkUploader(IMediator mediator, IBulkUploadValidator bulkUploadValidator, IBulkUploadFileParser fileParser, IProviderCommitmentsLogger logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (bulkUploadValidator == null)
                throw new ArgumentNullException(nameof(bulkUploadValidator));
            if (fileParser == null)
                throw new ArgumentNullException(nameof(fileParser));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _mediator = mediator;
            _bulkUploadValidator = bulkUploadValidator;
            _fileParser = fileParser;
            _logger = logger;
        }

        public async Task<BulkUploadResult> Validate(UploadApprenticeshipsViewModel uploadApprenticeshipsViewModel)
        {
            if (uploadApprenticeshipsViewModel.Attachment == null)
                return new BulkUploadResult { Errors = new List<UploadError> { new UploadError("No file chosen") } };

            var fileValidationErrors = _bulkUploadValidator.ValidateFileAttributes(uploadApprenticeshipsViewModel.Attachment).ToList();

            if (fileValidationErrors.Any())
            {
                foreach (var error in fileValidationErrors)
                    _logger.Warn($"  -->  {error.Message}");
                _logger.Warn($"Failed validation bulk upload file with {fileValidationErrors.Count} errors", providerId: uploadApprenticeshipsViewModel.ProviderId); // ToDo: Log what errors?

                return new BulkUploadResult { Errors = fileValidationErrors };
            }

            BulkUploadResult uploadResult = _fileParser.CreateViewModels(uploadApprenticeshipsViewModel.Attachment);

            if (uploadResult.Errors.Any())
                return uploadResult;

            var trainingProgrammes = GetTrainingProgrammes();
            var validationErrors = 
                _bulkUploadValidator.ValidateFields(uploadResult.Data, await trainingProgrammes, uploadApprenticeshipsViewModel.HashedCommitmentId).ToList();

            if (validationErrors.Any())
            {
                _logger.Warn($"Failed validation bulk upload records with {validationErrors.Count} errors", providerId: uploadApprenticeshipsViewModel.ProviderId); // ToDo: Log what errors?
                return new BulkUploadResult { Errors = validationErrors };
            }

            return new BulkUploadResult { Errors = new List<UploadError>(), Data = uploadResult .Data};
        }

        //TODO: These are duplicated in Commitment Orchestrator - needs to be shared
        private async Task<List<ITrainingProgramme>> GetTrainingProgrammes()
        {
            var standardsTask = _mediator.SendAsync(new GetStandardsQueryRequest());
            var frameworksTask = _mediator.SendAsync(new GetFrameworksQueryRequest());

            await Task.WhenAll(standardsTask, frameworksTask);

            return
                standardsTask.Result.Standards.Cast<ITrainingProgramme>()
                    .Union(frameworksTask.Result.Frameworks)
                    .OrderBy(m => m.Title)
                    .ToList();
        }
    }
}