using System;
using System.Collections.Generic;
using System.Linq;

using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class CsvRecordValidator : AbstractValidator<CsvRecord>
    {
        private static Func<int?, IEnumerable<int>, bool> _inList = (v, l) => !v.HasValue || l.Contains(v.Value);

        public CsvRecordValidator(BulkUploadApprenticeshipValidationText validationText)
        {
            // Only validating the fields that are in csv that dont have a 1-1 mapping in the domain model
            RuleFor(r => r.CohortRef)
                .NotNull().WithMessage(validationText.CohortRef01.Text).WithErrorCode(validationText.CohortRef01.ErrorCode)
                .NotEmpty().WithMessage(validationText.CohortRef02.Text).WithErrorCode(validationText.CohortRef02.ErrorCode)
                .Must(m => (m?.Length ?? 0) < 21).WithMessage(validationText.CohortRef02.Text).WithErrorCode(validationText.CohortRef02.ErrorCode);

            RuleFor(r => r.ProgType)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage(validationText.ProgType01.Text).WithErrorCode(validationText.ProgType01.ErrorCode)
                .LessThanOrEqualTo(99).WithMessage(validationText.ProgType01.Text).WithErrorCode(validationText.ProgType01.ErrorCode)
                .Must(m => _inList(m, new[] { 2, 3, 20, 21, 22, 23, 25 })).WithMessage(validationText.ProgType02.Text).WithErrorCode(validationText.ProgType02.ErrorCode);

            RuleFor(r => r.FworkCode)
                .LessThanOrEqualTo(999).WithMessage(validationText.FworkCode01.Text).WithErrorCode(validationText.FworkCode01.ErrorCode)
                .Must(m => (m ?? 0) > 0).When(m => m.ProgType.HasValue && _inList(m.ProgType, new[] { 2, 3, 20, 21, 22, 23 })).WithMessage(validationText.FworkCode02MustFworkCode.Text).WithErrorCode(validationText.FworkCode02MustFworkCode.ErrorCode);

            RuleFor(r => r.FworkCode)
                .Must(m => !m.HasValue || m.Value == 0).When(m => m.ProgType == 25).WithMessage(validationText.FworkCode03MustNotFworkCode.Text).WithErrorCode(validationText.FworkCode03MustNotFworkCode.ErrorCode);

            RuleFor(r => r.PwayCode)
                .LessThanOrEqualTo(999).When(m => _inList(m.ProgType, new[] { 2, 3, 20, 21, 22, 23 })).WithMessage(validationText.PwayCode01.Text).WithErrorCode(validationText.PwayCode01.ErrorCode)
                .Must(m => (m ?? 0) > 0).When(m => m.ProgType.HasValue && _inList(m.ProgType, new[] { 2, 3, 20, 21, 22, 23 })).WithMessage(validationText.PwayCode02.Text).WithErrorCode(validationText.PwayCode02.ErrorCode);
            RuleFor(r => r.PwayCode)
                .Must(m => !m.HasValue || m.Value == 0).When(m => m.ProgType == 25)
                    .WithMessage(validationText.PwayCode03.Text).WithErrorCode(validationText.PwayCode03.ErrorCode);

            RuleFor(r => r.StdCode)
                .LessThanOrEqualTo(99999).WithMessage(validationText.StdCode01.Text).WithErrorCode(validationText.StdCode01.ErrorCode)
                .Must(m => m.HasValue && m.Value > 0).When(m => m.ProgType == 25).WithMessage(validationText.StdCode02.Text).WithErrorCode(validationText.StdCode02.ErrorCode);

            RuleFor(r => r.StdCode)
                .Must(m => !m.HasValue || m.Value == 0).When(m => FrameworkProgTypeSelected(m))
                    .WithMessage(validationText.StdCode03.Text).WithErrorCode(validationText.StdCode03.ErrorCode);
        }

        private static bool FrameworkProgTypeSelected(CsvRecord record)
        {
            return record.ProgType.HasValue && _inList(record.ProgType, new int[] { 2, 3, 20, 21, 22, 23 }) ;
        }
    }
}