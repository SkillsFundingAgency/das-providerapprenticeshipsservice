using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation.Results;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;
using SFA.DAS.Learners.Validators;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public sealed class ApprenticeshipUploadModelValidator : IApprenticeshipUploadModelValidator
    {
        private readonly IApprenticeshipValidationErrorText _validationText;
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IUlnValidator _ulnValidator;
        private readonly IAcademicYearDateProvider _academicYearDateProvider;

        private static readonly Func<string, IEnumerable<string>, bool> InList = (v, l) => string.IsNullOrWhiteSpace(v) || l.Contains(v);

        public ApprenticeshipUploadModelValidator(IApprenticeshipValidationErrorText validationText, ICurrentDateTime currentDateTime, IUlnValidator ulnValidator, IAcademicYearDateProvider academicYearDateProvider)
        {
            _validationText = validationText;
            _currentDateTime = currentDateTime;
            _ulnValidator = ulnValidator;
            _academicYearDateProvider = academicYearDateProvider;
        }

        public ValidationResult Validate(ApprenticeshipUploadModel model)
        {
            var validationResult = new ValidationResult();
            ValidateField(validationResult, ValidateFirstName, model);
            ValidateField(validationResult, ValidateLastName, model);
            ValidateField(validationResult, ValidateULN, model);
            ValidateField(validationResult, ValidateDateOfBirth, model);
            ValidateField(validationResult, ValidateStartDate, model);
            ValidateField(validationResult, ValidateEndDate, model);
            ValidateField(validationResult, ValidateCost, model);
            ValidateField(validationResult, ValidateProviderRef, model);          
            ValidateField(validationResult, ValidateStdCode, model);
            ValidateField(validationResult, ValidateEmail, model);
            ValidateField(validationResult, ValidateAgreementId, model);
            return validationResult;
        }

        private ValidationFailure ValidateFirstName(ApprenticeshipUploadModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ApprenticeshipViewModel.FirstName))
            {
                return CreateValidationFailure("FirstName", _validationText.GivenNames01);
            }

            if (model.ApprenticeshipViewModel.FirstName.Length > 100)
            {
                return CreateValidationFailure("FirstName", _validationText.GivenNames02);
            }

            return null;
        }
            
        private ValidationFailure ValidateLastName(ApprenticeshipUploadModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ApprenticeshipViewModel.LastName))
            {
                return CreateValidationFailure("LastName", _validationText.FamilyName01);
            }

            if (model.ApprenticeshipViewModel.LastName.Length > 100)
            {
                return CreateValidationFailure("LastName", _validationText.FamilyName02);
            }

            return null;
        }

        private ValidationFailure ValidateULN(ApprenticeshipUploadModel model)
        {
            var result = _ulnValidator.Validate(model.ApprenticeshipViewModel.ULN);

            switch (result)
            {
                case UlnValidationResult.IsEmptyUlnNumber:
                case UlnValidationResult.IsInValidTenDigitUlnNumber:
                    return CreateValidationFailure("ULN", _validationText.Uln01);
                case UlnValidationResult.IsInvalidUln:
                    return CreateValidationFailure("ULN", _validationText.Uln03);
            }

            return null;
        }

        private ValidationFailure ValidateDateOfBirth(ApprenticeshipUploadModel model)
        {
            if (model.ApprenticeshipViewModel.DateOfBirth == null)
            {
                return CreateValidationFailure("DateOfBirth", _validationText.DateOfBirth01);
            }

            if (model.ApprenticeshipViewModel.DateOfBirth.DateTime == null || !model.ApprenticeshipViewModel.DateOfBirth.Day.HasValue)
            {
                return CreateValidationFailure("DateOfBirth", _validationText.DateOfBirth01);
            }

            if (!ApprenticeDobMustBeGreaterThenMinimumDob(model.ApprenticeshipViewModel.DateOfBirth))
            {
                return CreateValidationFailure("DateOfBirth", _validationText.DateOfBirth07);
            }

            if (!WillApprenticeBeAtLeast15AtStartOfTraining(model.ApprenticeshipViewModel, model.ApprenticeshipViewModel.DateOfBirth))
            {
                return CreateValidationFailure("DateOfBirth", _validationText.DateOfBirth02);
            }

            if (!ApprenticeAgeMustBeLessThen115AtStartOfTraining(model.ApprenticeshipViewModel, model.ApprenticeshipViewModel.DateOfBirth))
            {
                return CreateValidationFailure("DateOfBirth", _validationText.DateOfBirth06);
            }

            return null;
        }

        private bool ApprenticeDobMustBeGreaterThenMinimumDob(DateTimeViewModel dob)
        {
            DateTime? dobDate = dob?.DateTime;
            DateTime minimumDataOfBirth = new DateTime(1900, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            if (dobDate == null) return true;

            return dobDate > minimumDataOfBirth;
        }

        private ValidationFailure ValidateStartDate(ApprenticeshipUploadModel model)
        {
            if (model.ApprenticeshipViewModel.StartDate == null)
            {
                // we pass the field that failed, but that ultimatly gets discarded anyway, so why bother? we could just work with validationmessages
                return CreateValidationFailure("StartDate", _validationText.LearnStartDate01);
            }

            if (model.ApprenticeshipViewModel.StartDate.DateTime == null)
            {
                return CreateValidationFailure("StartDate", _validationText.LearnStartDate01);
            }

            var apprenticeshipAllowedStartDate = new DateTime(2017, 05, 01);
            if (model.ApprenticeshipViewModel.StartDate.DateTime < apprenticeshipAllowedStartDate
                && !model.ApprenticeshipViewModel.IsPaidForByTransfer)
                return CreateValidationFailure("StartDate", _validationText.LearnStartDate02);

            var transfersAllowedStartDate = new DateTime(2018, 05, 01);
            if (model.ApprenticeshipViewModel.StartDate.DateTime < transfersAllowedStartDate
                && model.ApprenticeshipViewModel.IsPaidForByTransfer)
                return CreateValidationFailure("StartDate", _validationText.LearnStartDate06);

            if (model.ApprenticeshipViewModel.StartDate.DateTime > _academicYearDateProvider.CurrentAcademicYearEndDate.AddYears(1))
                return CreateValidationFailure("StartDate", _validationText.LearnStartDate05);

            // we could check the start date against the training programme here, but we'd have to pass the trainingprogrammes through the call stack, or refetch them, or make them available another way e.g. static.
            // none of these choices are appealing, so we'll wait until bulk upload is refactored

            return null;
        }

        private ValidationFailure ValidateEndDate(ApprenticeshipUploadModel model)
        {
            if (model.ApprenticeshipViewModel.EndDate == null)
            {
                return CreateValidationFailure("EndDate", _validationText.LearnPlanEndDate01);
            }

            if (model.ApprenticeshipViewModel.EndDate.DateTime == null)
            {
                return CreateValidationFailure("EndDate", _validationText.LearnPlanEndDate01);
            }

            if (model.ApprenticeshipViewModel.StartDate != null && model.ApprenticeshipViewModel.EndDate.DateTime <= model.ApprenticeshipViewModel.StartDate.DateTime)
            {
                return CreateValidationFailure("EndDate", _validationText.LearnPlanEndDate02);
            }

            return null;
        }

        private ValidationFailure ValidateCost(ApprenticeshipUploadModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ApprenticeshipViewModel.Cost))
            {
                return CreateValidationFailure("Cost", _validationText.TrainingPrice01);
            }

            if (!Regex.Match(model.ApprenticeshipViewModel.Cost, "^([1-9]{1}([0-9]{1,2})?)+(,[0-9]{3})*$|^[1-9]{1}[0-9]*$").Success)
            {
                return CreateValidationFailure("Cost", _validationText.TrainingPrice01);
            }

            if (!decimal.TryParse(model.ApprenticeshipViewModel.Cost, out var parsed) || parsed > 100000)
            {
                return CreateValidationFailure("Cost", _validationText.TrainingPrice02);
            }

            return null;
        }

        private ValidationFailure ValidateProviderRef(ApprenticeshipUploadModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.ApprenticeshipViewModel.ProviderRef) && model.ApprenticeshipViewModel.ProviderRef.Length > 20)
            {
                return CreateValidationFailure("ProviderRef", _validationText.ProviderRef01);
            }

            return null;
        }
       
        private ValidationFailure ValidateStdCode(ApprenticeshipUploadModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CsvRecord.StdCode))
            {
                return CreateValidationFailure("StdCode", _validationText.StdCode04);
            }
            
            if (!string.IsNullOrWhiteSpace(model.CsvRecord.StdCode) && (model.CsvRecord.StdCode.TryParse() ?? 100000) > 99999)
            {
                return CreateValidationFailure("StdCode", _validationText.StdCode01);
            }

            if ((model.CsvRecord.StdCode.TryParse() ?? 0) <= 0)             
            {
                return CreateValidationFailure("StdCode", _validationText.StdCode02);
            }

            return null;
        }

        private ValidationFailure ValidateEmail(ApprenticeshipUploadModel model)
        {
            if (string.IsNullOrEmpty(model.CsvRecord.EmailAddress))
            {
                return CreateValidationFailure("EmailAddress", _validationText.EmailAddressBlank);
            }

            if (!model.CsvRecord.EmailAddress.IsAValidEmailAddress())
            {
                return CreateValidationFailure("EmailAddress", _validationText.EmailAddressNotValid);
            }

            if (model.CsvRecord.EmailAddress.Length > 200)
            {
                return CreateValidationFailure("EmailAddress", _validationText.EmailAddressLength);
            }

            return null;
        }

        private ValidationFailure ValidateAgreementId(ApprenticeshipUploadModel model)
        {
            if (string.IsNullOrEmpty(model.CsvRecord.AgreementId))
            {
                return CreateValidationFailure("AgreementId", _validationText.AgreementIdBlank);
            }
            
            return null;
        }

        private void ValidateField(ValidationResult validationResult, Func<ApprenticeshipUploadModel, ValidationFailure> validationFunc, ApprenticeshipUploadModel model)
        {
            var validationFailure = validationFunc(model);
            if (validationFailure != null)
            {
                validationResult.Errors.Add(validationFailure);
            }
        }

        private ValidationFailure CreateValidationFailure(string propertyName, ValidationMessage validationMessage)
        {
            return new ValidationFailure(propertyName, validationMessage.Text) {ErrorCode = validationMessage.ErrorCode};
        }

        private bool WillApprenticeBeAtLeast15AtStartOfTraining(ApprenticeshipViewModel model, DateTimeViewModel dob)
        {
            DateTime? startDate = model?.StartDate?.DateTime;
            DateTime? dobDate = dob?.DateTime;

            if (startDate == null || dob == null) return true; // Don't fail validation if both fields not set

            int age = startDate.Value.Year - dobDate.Value.Year;
            if (startDate < dobDate.Value.AddYears(age)) age--;

            return age >= 15;
        }

        private bool ApprenticeAgeMustBeLessThen115AtStartOfTraining(ApprenticeshipViewModel model, DateTimeViewModel dob)
        {
            DateTime? startDate = model?.StartDate?.DateTime;
            DateTime? dobDate = dob?.DateTime;

            if (startDate == null || dob == null) return true; // Don't fail validation if both fields not set

            int age = startDate.Value.Year - dobDate.Value.Year;
            if (startDate < dobDate.Value.AddYears(age)) age--;

            return age < 115;
        }

    }
}