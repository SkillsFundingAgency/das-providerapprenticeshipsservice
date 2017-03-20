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

        public async Task<BulkUploadResult> ValidateFileRows(IEnumerable<ApprenticeshipUploadModel> rows, long providerId)
        {
            var trainingProgrammes = await GetTrainingProgrammes();
            var validationErrors = _bulkUploadValidator.ValidateRecords(rows, trainingProgrammes).ToList();

            if (validationErrors.Any())
            {
                _logger.Warn($"Failed validation bulk upload records with {validationErrors.Count} errors", providerId);
                return new BulkUploadResult { Errors = validationErrors };
            }

            return new BulkUploadResult { Errors = new List<UploadError>(), Data = rows };
        }

        public BulkUploadResult ValidateFileStructure(UploadApprenticeshipsViewModel uploadApprenticeshipsViewModel, string filename, long commitmentId)
        {
            if (uploadApprenticeshipsViewModel.Attachment == null)
                return new BulkUploadResult { Errors = new List<UploadError> { new UploadError("No file chosen") } };

            var fileAttributeErrors = _bulkUploadValidator.ValidateFileSize(uploadApprenticeshipsViewModel.Attachment).ToList();

            if (fileAttributeErrors.Any())
            {
                foreach (var error in fileAttributeErrors)
                    _logger.Warn($"File Structure Error  -->  {error.Message}", providerId: uploadApprenticeshipsViewModel.ProviderId, commitmentId: commitmentId);

                _logger.Info($"Failed validation bulk upload file with {fileAttributeErrors.Count} errors", uploadApprenticeshipsViewModel.ProviderId, commitmentId: commitmentId);

                return new BulkUploadResult { Errors = fileAttributeErrors };
            }

            BulkUploadResult uploadResult = _fileParser.CreateViewModels(uploadApprenticeshipsViewModel.Attachment);

            if (uploadResult.HasErrors)
                return uploadResult;

            var errors = _bulkUploadValidator.ValidateCohortReference(uploadResult.Data, uploadApprenticeshipsViewModel.HashedCommitmentId).ToList();
            errors.AddRange(_bulkUploadValidator.ValidateUlnUniqueness(uploadResult.Data).ToList());

            return new BulkUploadResult { Errors = errors, Data = uploadResult.Data };
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