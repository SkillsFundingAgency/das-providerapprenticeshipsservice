using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
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
        private readonly ApprenticeshipBulkUploadValidator _viewModelValidator = new ApprenticeshipBulkUploadValidator();
        private readonly ApprenticeshipViewModelApproveValidator _approvalValidator = new ApprenticeshipViewModelApproveValidator();
        private readonly CsvRecordValidator _csvRecordValidator = new CsvRecordValidator();

        public BulkUploadValidator(ProviderApprenticeshipsServiceConfiguration config, ILog logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            _logger = logger;
            _config = config;
        }

        public IEnumerable<UploadError> ValidateFileAttributes(HttpPostedFileBase attachment)
        {
            var errors = new List<UploadError>();
            var maxFileSize = _config.MaxBulkUploadFileSize * 1024; // Bytes

            var regex = new Regex(@"\d{8}-\d{6}");
            var dateMatch = regex.Match(attachment.FileName);
            DateTime outDateTime;
            var dateParseSuccess = DateTime.TryParseExact(dateMatch.Value, "yyyyMMdd-HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out outDateTime);

            if (!dateMatch.Success || !dateParseSuccess || !Regex.IsMatch(attachment.FileName, @"APPDATA-\d{8}-\d{6}.csv"))
                errors.Add(new UploadError(ApprenticeshipFileValidationText.FilenameFormat.Text, ApprenticeshipFileValidationText.FilenameFormat.ErrorCode));
            else if(outDateTime > DateTime.Now)
                errors.Add(new UploadError(ApprenticeshipFileValidationText.FilenameFormatDate.Text, ApprenticeshipFileValidationText.FilenameFormatDate.ErrorCode));
            
            if (attachment.ContentLength > maxFileSize)
                errors.Add(new UploadError(ApprenticeshipFileValidationText.MaxFileSizeMessage(maxFileSize)));

            return errors;
        }

        public IEnumerable<UploadError> ValidateFields(IEnumerable<ApprenticeshipUploadModel> records, List<ITrainingProgramme> trainingProgrammes, string cohortReference)
        {
            var errors = new ConcurrentBag<UploadError>();

            var apprenticeshipUploadModels = records as ApprenticeshipUploadModel[] ?? records.ToArray();
            if (!apprenticeshipUploadModels.Any()) return new[] { new UploadError(ApprenticeshipFileValidationText.NoRecords) };

            if (apprenticeshipUploadModels.Any(m => m.CsvRecord.CohortRef != apprenticeshipUploadModels.First().CsvRecord.CohortRef))
                errors.Add(new UploadError("The Cohort Reference must be the same for all learners in the file", "CohortRef_03"));

            if (apprenticeshipUploadModels.Any(m => m.CsvRecord.CohortRef != cohortReference))
                errors.Add(new UploadError("The Cohort Reference does not match the current cohort", "CohortRef_04"));

            Parallel.ForEach(apprenticeshipUploadModels,
                (record, state, index) =>
                    {
                        var viewModel = record.ApprenticeshipViewModel;
                        int i = (int)index + 1;

                        // Validate view model for approval
                        var validationResult = _viewModelValidator.Validate(viewModel);
                        validationResult.Errors.ForEach(m => errors.Add(new UploadError(m.ErrorMessage, m.ErrorCode, i)));

                        var approvalValidationResult = _approvalValidator.Validate(viewModel);
                        approvalValidationResult.Errors.ForEach(m => errors.Add(new UploadError(m.ErrorMessage, m.ErrorCode, i)));

                        // Validate csv record
                        var csvValidationResult = _csvRecordValidator.Validate(record.CsvRecord);
                        csvValidationResult.Errors.ForEach(m => errors.Add(new UploadError(m.ErrorMessage, m.ErrorCode, i)));

                        if (!string.IsNullOrWhiteSpace(viewModel.TrainingCode) && trainingProgrammes.All(m => m.Id != viewModel.TrainingCode))
                            errors.Add(new UploadError("Not a valid training code", "StdCode_04", i));
                    });

            return errors;
        }
    }
}