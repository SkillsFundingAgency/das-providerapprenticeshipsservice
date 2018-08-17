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

        private static readonly Func<string, IEnumerable<string>, bool> InList = (v, l) => string.IsNullOrWhiteSpace(v) || l.Contains(v);

        public ApprenticeshipUploadModelValidator(IApprenticeshipValidationErrorText validationText, ICurrentDateTime currentDateTime, IUlnValidator ulnValidator)
        {
            _validationText = validationText;
            _currentDateTime = currentDateTime;
            _ulnValidator = ulnValidator;
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
            ValidateField(validationResult, ValidateProgType, model);
            ValidateField(validationResult, ValidateFworkCode, model);
            ValidateField(validationResult, ValidatePwayCode, model);
            ValidateField(validationResult, ValidateStdCode, model);
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

            if (!WillApprenticeBeAtLeast15AtStartOfTraining(model.ApprenticeshipViewModel, model.ApprenticeshipViewModel.DateOfBirth))
            {
                return CreateValidationFailure("DateOfBirth", _validationText.DateOfBirth02);
            }

            return null;
        }

        private ValidationFailure ValidateStartDate(ApprenticeshipUploadModel model)
        {
            if (model.ApprenticeshipViewModel.StartDate == null)
            {
                return CreateValidationFailure("StartDate", _validationText.LearnStartDate01);
            }

            if (model.ApprenticeshipViewModel.StartDate.DateTime == null)
            {
                return CreateValidationFailure("StartDate", _validationText.LearnStartDate01);
            }

            if (model.ApprenticeshipViewModel.StartDate.DateTime < new DateTime(2017, 5, 1))
            {
                return CreateValidationFailure("StartDate", _validationText.LearnStartDate02);
            }
            
            // we pass the field that failed, but that ultimatly gets discarded anyway, so why bother? we could just work with validationmessages anyway
            // to do it here, we'd have to pass the trainingprogrammes through the path to get here, or refetch them, or make them available another way e.g. static, none of these choices are appealing, so we'll wait until the bulk upload is refactored
            //if (!string.IsNullOrWhiteSpace(model.ApprenticeshipViewModel.TrainingCode))

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

            if (model.ApprenticeshipViewModel.EndDate.DateTime <= _currentDateTime.Now)
            {
                return CreateValidationFailure("EndDate", _validationText.LearnPlanEndDate03);
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
            if (!string.IsNullOrWhiteSpace(model.CsvRecord.StdCode) && (model.CsvRecord.StdCode.TryParse() ?? 100000) > 99999)
            {
                return CreateValidationFailure("StdCode", _validationText.StdCode01);
            }

            if (model.CsvRecord.ProgType == "25" && (model.CsvRecord.StdCode.TryParse() ?? 0) <= 0)
            {
                return CreateValidationFailure("StdCode", _validationText.StdCode02);
            }

            if (FrameworkProgTypeSelected(model.CsvRecord) && !string.IsNullOrWhiteSpace(model.CsvRecord.StdCode) && model.CsvRecord.StdCode != "0")
            {
                return CreateValidationFailure("StdCode", _validationText.StdCode03);
            }

            return null;
        }

        private ValidationFailure ValidatePwayCode(ApprenticeshipUploadModel model)
        {
            if(!string.IsNullOrWhiteSpace(model.CsvRecord.PwayCode) && InList(model.CsvRecord.ProgType, new[] { "2", "3", "20", "21", "22", "23" }) && model.CsvRecord.PwayCode.TryParse() > 999)
            {
                return CreateValidationFailure("PwayCode", _validationText.PwayCode01);
            }

            if(!string.IsNullOrWhiteSpace(model.CsvRecord.ProgType) && InList(model.CsvRecord.ProgType, new[] { "2", "3", "20", "21", "22", "23" }) && (model.CsvRecord.PwayCode.TryParse() ?? 0) <= 0)
            {
                return CreateValidationFailure("PwayCode", _validationText.PwayCode02);
            }

            if (model.CsvRecord.ProgType == "25" && !string.IsNullOrWhiteSpace(model.CsvRecord.PwayCode) && model.CsvRecord.PwayCode != "0")
            {
                return CreateValidationFailure("PwayCode", _validationText.PwayCode03);
            }

            return null;
        }

        private ValidationFailure ValidateFworkCode(ApprenticeshipUploadModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.CsvRecord.FworkCode) && (model.CsvRecord.FworkCode.TryParse() ?? 1000) > 999)
            {
                return CreateValidationFailure("FworkCode", _validationText.FworkCode01);
            }

            if (!string.IsNullOrWhiteSpace(model.CsvRecord.ProgType) && InList(model.CsvRecord.ProgType, new[] { "2", "3", "20", "21", "22", "23" }) && (model.CsvRecord.FworkCode.TryParse() ?? 0) <= 0)
            {
                return CreateValidationFailure("FworkCode", _validationText.FworkCode02);
            }

            if (model.CsvRecord.ProgType == "25" && !string.IsNullOrWhiteSpace(model.CsvRecord.FworkCode) && model.CsvRecord.FworkCode != "0")
            {
                return CreateValidationFailure("FworkCode", _validationText.FworkCode03);
            }

            return null;
        }

        private ValidationFailure ValidateProgType(ApprenticeshipUploadModel model)
        {
            if ((model.CsvRecord.ProgType.TryParse() ?? 100) > 99)
            {
                return CreateValidationFailure("ProgType", _validationText.ProgType01);
            }

            if (!InList(model.CsvRecord.ProgType, new[] { "2", "3", "20", "21", "22", "23", "25" }))
            {
                return CreateValidationFailure("ProgType", _validationText.ProgType02);
            }

            return null;
        }

        private static bool FrameworkProgTypeSelected(CsvRecord record)
        {
            return !string.IsNullOrWhiteSpace(record.ProgType) && InList(record.ProgType, new[] { "2", "3", "20", "21", "22", "23" });
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

    }
}