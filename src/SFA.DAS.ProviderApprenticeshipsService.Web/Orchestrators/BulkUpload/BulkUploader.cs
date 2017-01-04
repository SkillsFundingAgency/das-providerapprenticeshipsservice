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

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload
{
    public class BulkUploader
    {
        private readonly IMediator _mediator;

        private readonly BulkUploadValidator _bulkUploadValidator;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public BulkUploader(IMediator mediator, BulkUploadValidator bulkUploadValidator)
        {
            _mediator = mediator;
            _bulkUploadValidator = bulkUploadValidator;
        }

        public async Task<BulkUploadResult> UploadFile(UploadApprenticeshipsViewModel uploadApprenticeshipsViewModel)
        {
            if (uploadApprenticeshipsViewModel.Attachment == null)
                return new BulkUploadResult { Errors = new List<UploadError> { new UploadError("No file chosen") } };

            var fileValidationErrors = _bulkUploadValidator.ValidateFile(uploadApprenticeshipsViewModel.Attachment).ToList();
            if (fileValidationErrors.Any())
            {
                _logger.Warn($"Failed validation bulk upload file with {fileValidationErrors.Count} errors"); // ToDo: Log what errors?
                return new BulkUploadResult { Errors = fileValidationErrors };
            }

            BulkUploadResult uploadResult = _bulkUploadValidator.CreateViewModels(uploadApprenticeshipsViewModel.Attachment);
            if (uploadResult.Errors.Any()) return uploadResult;

            var trainingProgrammes = GetTrainingProgrammes();
            var validationErrors = 
                _bulkUploadValidator.ValidateFields(uploadResult.Data, await trainingProgrammes, uploadApprenticeshipsViewModel.HashedCommitmentId).ToList();

            if (validationErrors.Any())
            {
                _logger.Warn($"Failed validation bulk upload records with {validationErrors.Count} errors"); // ToDo: Log what errors?
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