using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using MediatR;
using NLog;

using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class BulkUploadOrchestrator
    {
        private readonly IMediator _mediator;

        private readonly BulkUploader _bulkUploader;

        private readonly IHashingService _hashingService;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public BulkUploadOrchestrator(IMediator mediator, BulkUploader bulkUploader, IHashingService hashingService)
        {
            _mediator = mediator;
            _bulkUploader = bulkUploader;
            _hashingService = hashingService;
        }

        public async Task<BulkUploadResult> UploadFile(UploadApprenticeshipsViewModel uploadApprenticeshipsViewModel)
        {
            if (uploadApprenticeshipsViewModel.Attachment == null)
            {
                return new BulkUploadResult { Errors = new List<UploadError> { new UploadError("No file chosen") } };
            }

            var fileValidationErrors = _bulkUploader.ValidateFile(uploadApprenticeshipsViewModel.Attachment).ToList();
            if (fileValidationErrors.Any())
            {
                _logger.Warn($"Failed validation bulk upload file with {fileValidationErrors.Count} errors"); // ToDo: Log what errors?
                return new BulkUploadResult {Errors = fileValidationErrors };
            }

            IEnumerable<ApprenticeshipUploadModel> data;
            try
            {
                data = _bulkUploader.CreateViewModels(uploadApprenticeshipsViewModel.Attachment);
            }
            catch (Exception exception)
            {
                // Move to bulkUpload
                return new BulkUploadResult { Errors = new List<UploadError> {new UploadError(exception.Message) } };
            }

            var trainingProgrammes = GetTrainingProgrammes();
            var validationErrors = _bulkUploader.ValidateFields(data, await trainingProgrammes).ToList();

            if (validationErrors.Any())
            {
                _logger.Warn($"Failed validation bulk upload records with {validationErrors.Count()} errors"); // ToDo: Log what errors?
                return new BulkUploadResult { Errors = validationErrors };
            }

            // ToDo: Send date to commitment _repository.uploadData(data);

            return new BulkUploadResult { Errors = new List<UploadError>() };
        }

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

        public async Task<UploadApprenticeshipsViewModel> GetUploadModel(long providerid, string hashedcommitmentid)
        {
            var commitmentId = _hashingService.DecodeValue(hashedcommitmentid);
            var result = await _mediator.SendAsync(new GetCommitmentQueryRequest
                                              {
                                                  ProviderId = providerid,
                                                  CommitmentId = commitmentId
                                              });

            var model = new UploadApprenticeshipsViewModel
            {
                ProviderId = providerid,
                HashedCommitmentId = hashedcommitmentid,
                ApprenticeshipCount = result.Commitment.Apprenticeships.Count
            };

            return model;
        }
    }
}