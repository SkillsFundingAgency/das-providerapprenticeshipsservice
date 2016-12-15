using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using NLog;

using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

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

        public IEnumerable<string> UploadFile(UploadApprenticeshipsViewModel uploadApprenticeshipsViewModel)
        {
            var fileValidationErrors = _bulkUploader.ValidateFile(uploadApprenticeshipsViewModel.Attachment).ToList();
            if (fileValidationErrors.Any())
            {
                logger.Warn($"Failed validation bulk upload file with {fileValidationErrors.Count} errors"); // ToDo: Log what errors?
                return fileValidationErrors;
            }

            // ToDo: Try catch on CsvHelper.CsvMissingFieldException AND CsvMissingFieldException
            var data = _bulkUploader.CreateViewModels(uploadApprenticeshipsViewModel.Attachment);

            var trainingProgrammes = GetTrainingProgrammes().Result;
            var validationErrors = _bulkUploader.ValidateFields(data, trainingProgrammes).ToList();
            if (validationErrors.Any())
            {
                logger.Warn($"Failed validation bulk upload records with {validationErrors.Count} errors"); // ToDo: Log what errors?
                return validationErrors;
            }

            // ToDo: Send date to commitment _repository.uploadData(data);

            // Create new view Model with errors if any
            return new List<string>();
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