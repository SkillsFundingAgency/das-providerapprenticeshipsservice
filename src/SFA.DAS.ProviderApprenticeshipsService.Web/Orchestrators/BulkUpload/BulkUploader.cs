﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SaveBulkUploadFile;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;
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
            _mediator = mediator;
            _bulkUploadValidator = bulkUploadValidator;
            _fileParser = fileParser;
            _logger = logger;
        }

        public async Task<BulkUploadResult> ValidateFileRows(IEnumerable<ApprenticeshipUploadModel> rows, long providerId, long bulkUploadId)
        {
            var trainingProgrammes = await GetTrainingProgrammes();
            var validationErrors = _bulkUploadValidator.ValidateRecords(rows, trainingProgrammes).ToList();

            if (validationErrors.Any())
            {
                var logtext = new StringBuilder();
                logtext.AppendLine($"Failed validation of bulk upload id {bulkUploadId} with {validationErrors.Count} errors");

                var errorTypes = validationErrors.GroupBy(x => x.ErrorCode);
                foreach (var errorType in errorTypes)
                {
                    var errorsOfType = validationErrors.FindAll(x => x.ErrorCode == errorType.Key);
                    logtext.AppendLine($"{errorsOfType.Count} x {errorType.Key} - \"{StripHtml(errorsOfType.First().Message)}\"");
                }

                _logger.Warn(logtext.ToString(), providerId);

                return new BulkUploadResult { Errors = validationErrors };
            }

            return new BulkUploadResult { Errors = new List<UploadError>(), Data = rows };
        }

        public async Task<BulkUploadResult> ValidateFileStructure(UploadApprenticeshipsViewModel uploadApprenticeshipsViewModel, long providerId, CommitmentView commitment)
        {
            if (uploadApprenticeshipsViewModel.Attachment == null)
                return new BulkUploadResult { Errors = new List<UploadError> { new UploadError("No file chosen") } };

            var fileContent = new StreamReader(uploadApprenticeshipsViewModel.Attachment.InputStream).ReadToEnd();
            var fileName = uploadApprenticeshipsViewModel?.Attachment?.FileName ?? "<- NO NAME ->";

            _logger.Trace($"Saving bulk upload file. {fileName}");
            var bulkUploadId = await _mediator.Send(
                new SaveBulkUploadFileCommand
                {
                    ProviderId = uploadApprenticeshipsViewModel.ProviderId,
                    CommitmentId = commitment.Id,
                    FileContent = fileContent,
                    FileName = fileName
                });
            _logger.Info($"Saved bulk upload with Id: {bulkUploadId}");

            var fileAttributeErrors = _bulkUploadValidator.ValidateFileSize(uploadApprenticeshipsViewModel.Attachment).ToList();

            if (fileAttributeErrors.Any())
            {
                foreach (var error in fileAttributeErrors)
                    _logger.Warn($"File Structure Error  -->  {error.Message}", uploadApprenticeshipsViewModel.ProviderId, commitment.Id);

                _logger.Info($"Failed validation bulk upload file with {fileAttributeErrors.Count} errors", uploadApprenticeshipsViewModel.ProviderId, commitment.Id);

                return new BulkUploadResult { Errors = fileAttributeErrors };
            }

            var uploadResult = _fileParser.CreateViewModels(providerId, commitment, fileContent);

            if (uploadResult.HasErrors)
                return uploadResult;

            var errors = _bulkUploadValidator.ValidateCohortReference(uploadResult.Data, uploadApprenticeshipsViewModel.HashedCommitmentId).ToList();
            errors.AddRange(_bulkUploadValidator.ValidateUlnUniqueness(uploadResult.Data).ToList());

            return new BulkUploadResult
            {
                Errors = errors,
                Data = uploadResult.Data,
                BulkUploadId = bulkUploadId
            };
        }

        private async Task<List<TrainingProgramme>> GetTrainingProgrammes()
        {
            var programmes = await _mediator.Send(new GetTrainingProgrammesQueryRequest
            {
                IncludeFrameworks = true
            });
            return programmes.TrainingProgrammes;
        }

        private string StripHtml(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }
    }
}