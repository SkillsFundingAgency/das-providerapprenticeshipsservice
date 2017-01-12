using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    public sealed class InstrumentedBulkUploadValidator : IBulkUploadValidator
    {
        private readonly ILog _logger;
        private readonly IBulkUploadValidator _validator;
        private readonly ApprenticeshipBulkUploadValidator _viewModelValidator = new ApprenticeshipBulkUploadValidator();
        private readonly ApprenticeshipViewModelApproveValidator _approvalValidator = new ApprenticeshipViewModelApproveValidator();
        private readonly CsvRecordValidator _csvRecordValidator = new CsvRecordValidator();

        public InstrumentedBulkUploadValidator(ILog logger, IBulkUploadValidator validator)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            _logger = logger;
            _validator = validator;
        }

        public IEnumerable<UploadError> ValidateFileAttributes(HttpPostedFileBase attachment)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = _validator.ValidateFileAttributes(attachment);

            _logger.Debug($"Took {stopwatch.ElapsedMilliseconds} milliseconds to validate file attributes");

            return result;
        }

        public IEnumerable<UploadError> ValidateFields(IEnumerable<ApprenticeshipUploadModel> records, List<ITrainingProgramme> trainingProgrammes, string cohortReference)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = _validator.ValidateFields(records, trainingProgrammes, cohortReference);

            _logger.Debug($"Took {stopwatch.ElapsedMilliseconds} milliseconds to validate fields for {records.Count()} items");

            return result;
        }
    }
}