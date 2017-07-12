using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload
{
    public sealed class BulkUploadFileParser : IBulkUploadFileParser
    {
        private readonly IProviderCommitmentsLogger _logger;

        public BulkUploadFileParser(IProviderCommitmentsLogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        public BulkUploadResult CreateViewModels(long providerId, long commitmentId, string fileInput)
        {
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
                        $"Failed to process bulk upload file (missing field). {exceptionData}", providerId: providerId, commitmentId: commitmentId);
                    return new BulkUploadResult { Errors = new List<UploadError> { new UploadError("Some mandatory fields are incomplete. Please check your file and upload again.") } };
                }
                catch (Exception exception)
                {
                    var errorMessage = "Upload failed. Please check your file and try again.";
                    var exceptionData = exception.Data["CsvHelper"];
                    _logger.Error(
                        exception,
                        $"Failed to process bulk upload file. {exceptionData}", providerId: providerId, commitmentId: commitmentId);

                    return new BulkUploadResult { Errors = new List<UploadError> { new UploadError(errorMessage) } };
                }
            }
        }

        private ApprenticeshipUploadModel MapTo(CsvRecord record)
        {
            var dateOfBirth = GetValidDate(record.DateOfBirth, "yyyy-MM-dd");
            var learnerStartDate = GetValidDate(record.StartDate, "yyyy-MM");
            var learnerEndDate = GetValidDate(record.EndDate, "yyyy-MM");

            var trainingCode = record.ProgType == "25"
                                   ? record.StdCode
                                   : $"{record.FworkCode}-{record.ProgType}-{record.PwayCode}";

            var apprenticeshipViewModel = new ApprenticeshipViewModel
            {
                AgreementStatus = AgreementStatus.NotAgreed,
                PaymentStatus = PaymentStatus.Active,
                ULN = record.ULN,
                FirstName = record.GivenNames,
                LastName = record.FamilyName,
                DateOfBirth = new DateTimeViewModel(dateOfBirth),
                Cost = record.TotalPrice,
                ProviderRef = record.ProviderRef,
                StartDate = new DateTimeViewModel(learnerStartDate),
                EndDate = new DateTimeViewModel(learnerEndDate),
                ProgType = record.ProgType.TryParse(),
                TrainingCode = trainingCode
            };
            return new ApprenticeshipUploadModel
            {
                ApprenticeshipViewModel = apprenticeshipViewModel,
                CsvRecord = record
            };
        }

        private DateTime? GetValidDate(string date, string format)
        {
            DateTime outDateTime;
            if (DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out outDateTime))
                return outDateTime;
            return null;
        }
    }
}