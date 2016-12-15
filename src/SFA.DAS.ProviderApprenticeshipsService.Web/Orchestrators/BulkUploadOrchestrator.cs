using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using NLog;

using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class BulkUploadOrchestrator
    {
        private readonly IMediator _mediator;

        private readonly BulkUploader _bulkUploader;

        private ILogger logger = LogManager.GetCurrentClassLogger();

        public BulkUploadOrchestrator(BulkUploader bulkUploader)
        {
            _bulkUploader = bulkUploader;
        }

        public BulkUploadResult UploadFile(UploadApprenticeshipsViewModel uploadApprenticeshipsViewModel)
        {
            var fileValidationErrors = _bulkUploader.ValidateFile(uploadApprenticeshipsViewModel.Attachment).ToList();
            if (fileValidationErrors.Any())
            {
                logger.Warn($"Failed validation bulk upload file with {fileValidationErrors.Count} errors"); // ToDo: Log what errors?
                return new BulkUploadResult {Errors = fileValidationErrors };
            }

            IEnumerable<ApprenticeshipViewModel> data;
            try
            {
                data = _bulkUploader.CreateViewModels(uploadApprenticeshipsViewModel.Attachment);
            }
            catch (Exception exception)
            {
                return new BulkUploadResult { Errors = new List<UploadError> {new UploadError(exception.Message) } };
            }

            var trainingProgrammes = GetTrainingProgrammes().Result;
            var validationErrors = _bulkUploader.ValidateFields(data, trainingProgrammes).ToList();
            if (validationErrors.Any())
            {
                logger.Warn($"Failed validation bulk upload records with {validationErrors.Count} errors"); // ToDo: Log what errors?
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
                    .Union(frameworksTask.Result.Frameworks.Cast<ITrainingProgramme>())
                    .OrderBy(m => m.Title)
                    .ToList();
        }
    }
}