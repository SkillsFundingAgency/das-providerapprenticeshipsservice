using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

using WebGrease.Css.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload
{
    public sealed class BulkUploadValidator : IBulkUploadValidator
    {
        private readonly ILog _logger;
        private readonly ProviderApprenticeshipsServiceConfiguration _config;

        // TODO: LWA - Can these be injected in?
        private readonly BulkUploadApprenticeshipValidationText _validationText;
        private readonly ApprenticeshipBulkUploadValidator _viewModelValidator;
        private readonly CsvRecordValidator _csvRecordValidator;

        public BulkUploadValidator(ProviderApprenticeshipsServiceConfiguration config, ILog logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            _validationText = new BulkUploadApprenticeshipValidationText();
            _viewModelValidator = new ApprenticeshipBulkUploadValidator(_validationText, new CurrentDateTime());
            _csvRecordValidator = new CsvRecordValidator(_validationText);

            _logger = logger;
            _config = config;
        }

        public IEnumerable<UploadError> ValidateFileSize(HttpPostedFileBase attachment)
        {
            var errors = new List<UploadError>();
            var maxFileSize = _config.MaxBulkUploadFileSize * 1024; // Bytes
            
            if (attachment.ContentLength > maxFileSize)
                errors.Add(new UploadError(ApprenticeshipFileValidationText.MaxFileSizeMessage(maxFileSize)));

            return errors;
        }

        public IEnumerable<UploadError> ValidateCohortReference(
            IEnumerable<ApprenticeshipUploadModel> records,
            string cohortReference)
        {
            var errors = new List<UploadError>();

            var apprenticeshipUploadModels = records as ApprenticeshipUploadModel[] ?? records.ToArray();
            if (!apprenticeshipUploadModels.Any()) return new[] { new UploadError(ApprenticeshipFileValidationText.NoRecords) };

            if (apprenticeshipUploadModels.Any(m => m.CsvRecord.CohortRef != apprenticeshipUploadModels.First().CsvRecord.CohortRef))
                errors.Add(new UploadError(_validationText.CohortRef01.Text, _validationText.CohortRef01.ErrorCode));

            if (apprenticeshipUploadModels.Any(m => m.CsvRecord.CohortRef != cohortReference))
                errors.Add(new UploadError(_validationText.CohortRef02.Text, _validationText.CohortRef02.ErrorCode));

            return errors;
        }

        public IEnumerable<UploadError> ValidateRecords(IEnumerable<ApprenticeshipUploadModel> records, List<ITrainingProgramme> trainingProgrammes)
        {
            var errors = new ConcurrentBag<UploadError>();
            var apprenticeshipUploadModels = records as ApprenticeshipUploadModel[] ?? records.ToArray();
            
            Parallel.ForEach(apprenticeshipUploadModels,
                (record, state, index) =>
                    {
                        var viewModel = record.ApprenticeshipViewModel;
                        int i = (int)index + 1;

                        // Validate view model for approval
                        var validationResult = _viewModelValidator.Validate(viewModel);
                        validationResult.Errors.ForEach(m => errors.Add(new UploadError(m.ErrorMessage, m.ErrorCode, i, record)));

                        // Validate csv record
                        var csvValidationResult = _csvRecordValidator.Validate(record.CsvRecord);
                        csvValidationResult.Errors.ForEach(m => errors.Add(new UploadError(m.ErrorMessage, m.ErrorCode, i, record)));

                        // TODO: LWA - Should we move this into the validator?
                        if (!string.IsNullOrWhiteSpace(viewModel.TrainingCode) && trainingProgrammes.All(m => m.Id != viewModel.TrainingCode))
                            errors.Add(new UploadError("Not a valid <strong>Training code</strong>", "Training_01", i, record));
                    });

            return errors;
        }
    }
}