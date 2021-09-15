using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Extensions;
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
            _logger = logger;
        }

        public BulkUploadResult CreateViewModels(long providerId, CommitmentView commitment, string fileInput, bool blackListed)
        {
            const string errorMessage = "Upload failed. Please check your file and try again.";          
            using (var tr = new StringReader(fileInput))
            {
                try
                {
                    if (string.IsNullOrEmpty(fileInput)) {                        
                        throw new Exception();
                    }

                    var csvReader = new CsvReader(tr);
                    csvReader.Configuration.HasHeaderRecord = true;
                    csvReader.Configuration.PrepareHeaderForMatch = (header, index) => header.ToLower();
                    csvReader.Configuration.BadDataFound = cont => throw new Exception("Bad data found");
                    if (blackListed)
                        csvReader.Configuration.RegisterClassMap<CsvRecordBlackListMap>(); 
                    else
                        csvReader.Configuration.RegisterClassMap<CsvRecordMap>();

                    return new BulkUploadResult
                    {
                        Data = csvReader.GetRecords<CsvRecord>()
                            .ToList()
                            .Select(record => MapTo(record, commitment, blackListed))
                    };
                }
                catch (HeaderValidationException)
                {   
                    _logger.Info("Failed to process bulk upload file (missing field).", providerId, commitment.Id);
                    return new BulkUploadResult { Errors = new List<UploadError> { new UploadError(errorMessage) } };
                }
                catch (Exception)
                {
                    _logger.Info("Failed to process bulk upload file.", providerId, commitment.Id);
                    return new BulkUploadResult { Errors = new List<UploadError> { new UploadError(errorMessage) } };
                }
            }
        }

        private ApprenticeshipUploadModel MapTo(CsvRecord record, CommitmentView commitment, bool blackListed)
        {          
            var dateOfBirth = GetValidDate(record.DateOfBirth, "yyyy-MM-dd");            
            var learnerStartDate = GetValidDate(record.StartDate, "yyyy-MM-dd");
            if (learnerStartDate != null)
                learnerStartDate = new DateTime(learnerStartDate.GetValueOrDefault().Year, learnerStartDate.GetValueOrDefault().Month, 1);
            var learnerEndDate = GetValidDate(record.EndDate, "yyyy-MM");
            
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
                CourseCode = record.StdCode,
                IsPaidForByTransfer = commitment.IsTransfer(),
                AgreementId = commitment.AccountLegalEntityPublicHashedId,
                EmailAddress = record.EmailAddress,
                BlackListed = blackListed
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