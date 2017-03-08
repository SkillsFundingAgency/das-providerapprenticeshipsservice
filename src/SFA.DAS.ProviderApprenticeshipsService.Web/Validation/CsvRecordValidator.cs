using System;
using System.Collections.Generic;
using System.Linq;

using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class CsvRecordValidator : AbstractValidator<CsvRecord>
    {
        private static Func<string, IEnumerable<string>, bool> _inList = (v, l) 
            => string.IsNullOrEmpty(v) || l.Contains(v);

        public CsvRecordValidator(BulkUploadApprenticeshipValidationText validationText)
        {
                // Only validating the fields that are in csv that dont have a 1-1 mapping in the domain model
            RuleFor(r => r.ProgType)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(m => m.TryParse() <= 99)
                    .WithMessage(validationText.ProgType01.Text).WithErrorCode(validationText.ProgType01.ErrorCode)
                .Must(m => _inList(m, new[] { "2", "3", "20", "21", "22", "23", "25" }))
                    .WithMessage(validationText.ProgType02.Text).WithErrorCode(validationText.ProgType02.ErrorCode);

            RuleFor(r => r.FworkCode)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(m => m.TryParse() <= 999).When(m => !string.IsNullOrEmpty(m.FworkCode))
                    .WithMessage(validationText.FworkCode01.Text).WithErrorCode(validationText.FworkCode01.ErrorCode)
                .Must(m => (m.TryParse() ?? 0) > 0).When(m => !string.IsNullOrEmpty(m.ProgType) && _inList(m.ProgType, new[] { "2", "3", "20", "21", "22", "23" })).WithMessage(validationText.FworkCode02.Text).WithErrorCode(validationText.FworkCode02.ErrorCode);

            RuleFor(r => r.FworkCode)
                .Must(m => string.IsNullOrEmpty(m) || m == "0").When(m => m.ProgType == "25").WithMessage(validationText.FworkCode03.Text).WithErrorCode(validationText.FworkCode03.ErrorCode);

            RuleFor(r => r.PwayCode)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(m => m.TryParse() <= 999)
                    .When(m => !string.IsNullOrEmpty(m.PwayCode) && _inList(m.ProgType, new[]{ "2","3","20","21","22","23" }))
                    .WithMessage(validationText.PwayCode01.Text).WithErrorCode(validationText.PwayCode01.ErrorCode)
                .Must(m => (m.TryParse() ?? 0) > 0).When(m => !string.IsNullOrEmpty(m.ProgType) && _inList(m.ProgType, new[] { "2", "3", "20", "21", "22", "23" })).WithMessage(validationText.PwayCode02.Text).WithErrorCode(validationText.PwayCode02.ErrorCode);

            RuleFor(r => r.PwayCode)
                .Must(m => string.IsNullOrEmpty(m) || m == "0").When(m => m.ProgType == "25")
                    .WithMessage(validationText.PwayCode03.Text).WithErrorCode(validationText.PwayCode03.ErrorCode);

            RuleFor(r => r.StdCode)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(m => m.TryParse() <= 99999).When(m => !string.IsNullOrEmpty(m.StdCode))
                    .WithMessage(validationText.StdCode01.Text).WithErrorCode(validationText.StdCode01.ErrorCode)
                .Must(m => m.TryParse() > 0).When(m => m.ProgType == "25").WithMessage(validationText.StdCode02.Text).WithErrorCode(validationText.StdCode02.ErrorCode);

            RuleFor(r => r.StdCode)
                .Must(m => string.IsNullOrEmpty(m) || m == "0").When(FrameworkProgTypeSelected)
                    .WithMessage(validationText.StdCode03.Text).WithErrorCode(validationText.StdCode03.ErrorCode);
        }

        private static bool FrameworkProgTypeSelected(CsvRecord record)
        {
            return !string.IsNullOrEmpty(record.ProgType) && _inList(record.ProgType, new[] { "2","3","20","21","22","23" }) ;
        }
    }
}