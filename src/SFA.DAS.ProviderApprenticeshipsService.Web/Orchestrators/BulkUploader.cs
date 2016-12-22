using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using CsvHelper;

using NLog;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

using WebGrease.Css.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class BulkUploader
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<UploadError> ValidateFile(HttpPostedFileBase attachment)
        {
            var errors = new List<UploadError>();
            var maxFileSize = 512 * 1000; // ToDo: Move to config
            var fileEnding = ".csv";
            var fileStart = "APPDATA";

            var regex = new Regex(@"\d{8}-\d{6}");
            var dateMatch = regex.Match(attachment.FileName);
            DateTime outDateTime;
            var dateParseSuccess = DateTime.TryParseExact(dateMatch.Value, "yyyyMMdd-HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out outDateTime);
            if (!dateMatch.Success || !dateParseSuccess)
                errors.Add(new UploadError(ApprenticeshipFileValidationText.FilenameFormat.Text, ApprenticeshipFileValidationText.FilenameFormat.ErrorCode));

            else if(outDateTime > DateTime.Now)
                errors.Add(new UploadError(ApprenticeshipFileValidationText.FilenameFormatDate.Text, ApprenticeshipFileValidationText.FilenameFormatDate.ErrorCode));

            else if (!attachment.FileName.EndsWith(fileEnding) || !attachment.FileName.StartsWith(fileStart))
                errors.Add(new UploadError(ApprenticeshipFileValidationText.FilenameFormat.Text, ApprenticeshipFileValidationText.FilenameFormat.ErrorCode));
            
            if (attachment.ContentLength > maxFileSize)
                errors.Add(new UploadError(ApprenticeshipFileValidationText.MaxFileSizeMessage(maxFileSize)));

            return errors;
        }

        public IEnumerable<ApprenticeshipUploadModel> CreateViewModels(HttpPostedFileBase attachment)
        {
            string result = new StreamReader(attachment.InputStream).ReadToEnd();
            using (TextReader tr = new StringReader(result))
            {
                var csvReader = new CsvReader(tr);
                csvReader.Configuration.HasHeaderRecord = true;
                csvReader.Configuration.IsHeaderCaseSensitive = false;

                try
                {
                    return csvReader.GetRecords<CsvRecord>()
                        .ToList()
                        .Select(MapTo);
                }
                catch (CsvMissingFieldException exception)
                {
                    var exceptionData = exception.Data["CsvHelper"];
                    _logger.Error(
                        exception,
                        $"Failed to create files from bulk upload. {typeof(CsvMissingFieldException)} Data CsvHelper {exceptionData}");
                    throw new Exception("Cannot read all file");
                }
                catch (Exception exception)
                {
                    var exceptionData = exception.Data["CsvHelper"];
                    _logger.Error(
                        exception,
                        $"Failed to create files from bulk upload. Exception Data CsvHelper {exceptionData}");
                    throw new Exception("Failed to create apprentices from file");
                }
            }
        }

        public virtual IEnumerable<UploadError> ValidateFields(IEnumerable<ApprenticeshipUploadModel> records, List<ITrainingProgramme> trainingProgrammes)
        {
            var errors = new List<UploadError>();

            if (!records.Any()) return new[] { new UploadError(ApprenticeshipFileValidationText.NoRecords) };

            var index = 1;
            foreach (var record in records)
            {
                var viewModel = record.ApprenticeshipViewModel;
                index++;

                var viewModelValidator = new ApprenticeshipBulkUploadValidator();
                var approvalValidator = new ApprenticeshipViewModelApproveValidator();
                var csvRecordValidator = new CsvRecordValidator();

                // Validate view model 
                var validationResult = viewModelValidator.Validate(viewModel);
                validationResult.Errors.ForEach(m => errors.Add(new UploadError(m.ErrorMessage, m.ErrorCode, index)));

                // Validate view model for approval
                var approvalValidationResult = approvalValidator.Validate(viewModel);
                approvalValidationResult.Errors.ForEach(m => errors.Add(new UploadError(m.ErrorMessage, m.ErrorCode, index)));

                // Validate csv record
                var csvValidationResult = csvRecordValidator.Validate(record.CsvRecord);
                csvValidationResult.Errors.ForEach(m => errors.Add(new UploadError(m.ErrorMessage, m.ErrorCode, index)));

                if (!string.IsNullOrWhiteSpace(viewModel.TrainingCode) && trainingProgrammes.All(m => m.Id != viewModel.TrainingCode))
                    errors.Add(new UploadError("Not a valid training code", "StdCode_04", index));
            }
            return errors;
        }

        private ApprenticeshipUploadModel MapTo(CsvRecord record)
        {
            var dateOfBirth = GetValidDate(record.DateOfBirth);
            var learnerStartDate = GetValidDate(record.LearnStartDate);
            var learnerEndDate = GetValidDate(record.LearnPlanEndDate);

            var trainingCode = record.ProgType == 25
                                   ? record.StdCode.ToString()
                                   : $"{record.FworkCode}-{record.ProgType}-{record.PwayCode}";

            var apprenticeshipViewModel = new ApprenticeshipViewModel
            {
                AgreementStatus = AgreementStatus.NotAgreed,
                PaymentStatus = PaymentStatus.Active,
                FirstName = record.GivenNames,
                LastName = record.FamilyName,
                DateOfBirth = new DateTimeViewModel(dateOfBirth),
                ULN = record.ULN.ToString(),
                NINumber = record.NINumber,
                Cost = record.TrainingPrice.ToString(),
                ProviderRef = record.ProvRef,
                StartDate = new DateTimeViewModel(learnerStartDate),
                EndDate = new DateTimeViewModel(learnerEndDate),
                ProgType = record.ProgType,
                TrainingCode = trainingCode
            };
            return new ApprenticeshipUploadModel
                       {
                           ApprenticeshipViewModel = apprenticeshipViewModel,
                           CsvRecord = record
                       };
        }

        private DateTime? GetValidDate(string date)
        {
            DateTime outDateTime;
            if (DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out outDateTime))
                return outDateTime;
            return null;
        }
    }
}