using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using CsvHelper;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload
{
    public sealed class BulkUploadFileParser : IBulkUploadFileParser
    {
        private readonly ILog _logger;

        public BulkUploadFileParser(ILog logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        public BulkUploadResult CreateViewModels(HttpPostedFileBase attachment)
        {
            string fileInput = new StreamReader(attachment.InputStream).ReadToEnd();
            using (TextReader tr = new StringReader(fileInput))
            {
                var csvReader = new CsvReader(tr);
                csvReader.Configuration.HasHeaderRecord = true;
                csvReader.Configuration.IsHeaderCaseSensitive = false;

                try
                {
                    return new BulkUploadResult
                    {
                        Data = csvReader.GetRecords<CsvRecord>()
                                    .ToList()
                                    .Select(MapTo)
                    };
                }
                catch (CsvMissingFieldException exception)
                {
                    var exceptionData = exception.Data["CsvHelper"];
                    _logger.Error(
                        exception,
                        $"Failed to create files from bulk upload. {typeof(CsvMissingFieldException)} Data CsvHelper {exceptionData}");
                    return new BulkUploadResult { Errors = new List<UploadError> { new UploadError("Cannot read all file") } };
                }
                catch (Exception exception)
                {
                    var exceptionData = exception.Data["CsvHelper"];
                    _logger.Error(
                        exception,
                        $"Failed to create files from bulk upload. Exception Data CsvHelper {exceptionData}");
                    return new BulkUploadResult { Errors = new List<UploadError> { new UploadError("Failed to create apprentices from file") } };
                }
            }
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