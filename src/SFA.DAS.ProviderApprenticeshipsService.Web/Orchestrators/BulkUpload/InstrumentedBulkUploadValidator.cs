using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.Learners.Validators;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload
{
    public sealed class InstrumentedBulkUploadValidator : IBulkUploadValidator
    {
        private readonly ILog _logger;
        private readonly IBulkUploadValidator _validator;
        
        public InstrumentedBulkUploadValidator(ILog logger, IBulkUploadValidator validator, IUlnValidator ulnValidator, IAcademicYearDateProvider academicYear)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            _logger = logger;
            _validator = validator;
        }

        public IEnumerable<UploadError> ValidateCohortReference(IEnumerable<ApprenticeshipUploadModel> records, string cohortReference)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = _validator.ValidateCohortReference(records, cohortReference);

            _logger.Trace($"Took {stopwatch.ElapsedMilliseconds} milliseconds to validate file for {records.Count()} items");

            return result;
        }

        public IEnumerable<UploadError> ValidateFileSize(HttpPostedFileBase attachment)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = _validator.ValidateFileSize(attachment);

            _logger.Debug($"Took {stopwatch.ElapsedMilliseconds} milliseconds to validate file attributes");

            return result;
        }

        public IEnumerable<UploadError> ValidateRecords(IEnumerable<ApprenticeshipUploadModel> records, List<TrainingProgramme> trainingProgrammes)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = _validator.ValidateRecords(records, trainingProgrammes);

            _logger.Trace($"Took {stopwatch.ElapsedMilliseconds} milliseconds to validate fields for {records.Count()} items");

            return result;
        }

        public IEnumerable<UploadError> ValidateUlnUniqueness(IEnumerable<ApprenticeshipUploadModel> records)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = _validator.ValidateUlnUniqueness(records);

            _logger.Trace($"Took {stopwatch.ElapsedMilliseconds} milliseconds to validate ULN uniqueness for {records.Count()} items");

            return result;
        }

        public IEnumerable<UploadError> ValidateAgreementId(IEnumerable<ApprenticeshipUploadModel> records, string agreementId)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = _validator.ValidateAgreementId(records, agreementId);

            _logger.Trace($"Took {stopwatch.ElapsedMilliseconds} milliseconds to validate AgreementID for {records.Count()} items");

            return result;
        }

        public IEnumerable<UploadError> ValidateEmailUniqueness(IEnumerable<ApprenticeshipUploadModel> records)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = _validator.ValidateEmailUniqueness(records);

            _logger.Trace($"Took {stopwatch.ElapsedMilliseconds} milliseconds to validate Email Address for {records.Count()} items");

            return result;
        }
    }
}