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
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

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
            if (!dateMatch.Success)
                errors.Add(new UploadError($"File name must include the date with fomat: yyyyMMdd-HHmmss", "Filename_01"));
            else if (!dateParseSuccess)
                errors.Add(new UploadError($"Date in file name is not valid", "Filename_01"));
            else if(outDateTime < DateTime.Now)
                errors.Add(new UploadError("Date and time must be bofore now", "Filename_02"));

            if (!attachment.FileName.EndsWith(fileEnding))
                errors.Add(new UploadError($"File name must end with {fileEnding}", "Filename_01"));
            if (!attachment.FileName.StartsWith(fileStart))
                errors.Add(new UploadError($"File name must start with {fileStart}", "Filename_01"));
            if (attachment.ContentLength > maxFileSize)
                errors.Add(new UploadError($"File size cannot be larger then {maxFileSize}"));

            return errors;
        }

        public IEnumerable<ApprenticeshipViewModel> CreateViewModels(HttpPostedFileBase attachment)
        {
            string result = new StreamReader(attachment.InputStream).ReadToEnd();
            using (TextReader tr = new StringReader(result))
            {
                var csvReader = new CsvReader(tr);
                csvReader.Configuration.HasHeaderRecord = true;

                try
                {
                    return csvReader.GetRecords<CsvRecords>().ToList().Select(MapTo);
                }
                catch (CsvMissingFieldException exception)
                {
                    var exceptionData = exception.Data["CsvHelper"];
                    _logger.Warn(
                        exception,
                        $"Failed to create files from bulk upload. {typeof(CsvMissingFieldException)} Data CsvHelper {exceptionData}");
                    throw new Exception("Cannot read all file");
                }
                catch (Exception exception)
                {
                    var exceptionData = exception.Data["CsvHelper"];
                    _logger.Warn(
                        exception,
                        $"Failed to create files from bulk upload. Exception Data CsvHelper {exceptionData}");
                    throw new Exception("Failed to create apprentices from file");
                }
            }
        }

        private ApprenticeshipViewModel MapTo(CsvRecords record)
        {
            var dateOfBirth = GetValidDate(record.DateOfBirth);
            var learnerStartDate = GetValidDate(record.LearnStartDate);
            var learnerEndDate = GetValidDate(record.LearnPlanEndDate);

            var trainingCode = record.StdCode != null
                                   ? record.StdCode.ToString()
                                   : $"{record.FworkCode}-{record.ProgType}-{record.PwayCode}"; // ToDo: Confirm // ProgType => 2,3,20,21,22,23,25

            var apprenticeshipViewModel = new ApprenticeshipViewModel
            {
                AgreementStatus = AgreementStatus.NotAgreed,
                PaymentStatus = PaymentStatus.Active,
                FirstName = record.GivenName,
                LastName = record.FamilyName,
                DateOfBirthYear = dateOfBirth?.Year,
                DateOfBirthMonth = dateOfBirth?.Month,
                DateOfBirthDay = dateOfBirth?.Day,
                ULN = record.ULN.ToString(),
                NINumber = record.NINumber,
                Cost = record.TrainingPrice.ToString(),
                ProviderRef = record.ProvRef,
                EmployerRef = record.EmpRef, // ToDo: Do we set employer ref?
                StartMonth = learnerStartDate?.Month,
                StartYear = learnerStartDate?.Year,
                EndMonth = learnerEndDate?.Month,
                EndYear = learnerEndDate?.Year,
                TrainingCode = trainingCode
            };
            return apprenticeshipViewModel;
        }

        public virtual IEnumerable<UploadError> ValidateFields(IEnumerable<ApprenticeshipViewModel> records, List<ITrainingProgramme> trainingProgrammes)
        {
            var errors = new List<UploadError>();

            if (!records.Any()) return new[] { new UploadError("File contains no records") };

            var index = 0;
            foreach (var record in records)
            {
                index++;

                var viewModelValidator = new ApprenticeshipViewModelValidator();
                var approvalValidator = new ApprenticeshipViewModelApproveValidator();

                // Validate view model 
                var validationResult = viewModelValidator.Validate(record);
                validationResult.Errors.ForEach(m => errors.Add(new UploadError(m.ErrorMessage, m.ErrorCode, index)));

                // Validate view model for approval
                var approvalValidationResult = approvalValidator.Validate(record);
                approvalValidationResult.Errors.ForEach(m => errors.Add(new UploadError(m.ErrorMessage, m.ErrorCode, index)));

                if(trainingProgrammes.All(m => m.Id != record.TrainingCode))
                    errors.Add(new UploadError($"Not a valid training code: {record.TrainingCode}")); // ToDo: Add error code StdCode_04
            }
            return errors;
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