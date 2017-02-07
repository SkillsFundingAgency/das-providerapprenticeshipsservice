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
        public CsvRecordValidator(BulkUploadApprenticeshipValidationText validationText)
        {
            Func<int?, IEnumerable<int>, bool> inList = (v, l) => !v.HasValue || l.Contains(v.Value);

            RuleFor(r => r.CohortRef)
                .NotNull().WithMessage(validationText.CohortRef01.Text).WithErrorCode(validationText.CohortRef01.ErrorCode)
                .NotEmpty().WithMessage(validationText.CohortRef02.Text).WithErrorCode(validationText.CohortRef02.ErrorCode)
                .Must(m => (m?.Length ?? 0) < 21).WithMessage(validationText.CohortRef02.Text).WithErrorCode(validationText.CohortRef02.ErrorCode);

            RuleFor(r => r.ProgType)
                .NotEmpty().WithMessage(validationText.ProgType01.Text).WithErrorCode(validationText.ProgType01.ErrorCode)
                .LessThanOrEqualTo(99).WithMessage(validationText.ProgType01.Text).WithErrorCode(validationText.ProgType01.ErrorCode)
                .Must(m => inList(m, new [] { 2, 3, 20, 21, 22, 23, 25})).WithMessage(validationText.ProgType02.Text).WithErrorCode(validationText.ProgType02.ErrorCode);

            RuleFor(r => r.FworkCode)
                .LessThanOrEqualTo(999).WithMessage(validationText.FworkCode01.Text).WithErrorCode(validationText.FworkCode01.ErrorCode)
                .Must(m => (m ?? 0) > 0).When(m => m.ProgType.HasValue && inList(m.ProgType, new[] { 2, 3, 20, 21, 22, 23 })).WithMessage(validationText.FworkCode02MustFworkCode.Text).WithErrorCode(validationText.FworkCode02MustFworkCode.ErrorCode);

            RuleFor(r => r.FworkCode)
                .Must(m => !m.HasValue || m.Value == 0).When(m => m.ProgType == 25).WithMessage(validationText.FworkCode03MustNotFworkCode.Text).WithErrorCode(validationText.FworkCode03MustNotFworkCode.ErrorCode);

            RuleFor(r => r.PwayCode)
                .LessThanOrEqualTo(999).When(m => inList(m.ProgType, new[] { 2, 3, 20, 21, 22, 23 })).WithMessage(validationText.PwayCode01.Text).WithErrorCode(validationText.PwayCode01.ErrorCode)
                .Must(m => (m ?? 0) > 0).When(m => m.ProgType.HasValue && inList(m.ProgType, new[] { 2, 3, 20, 21, 22, 23 })).WithMessage(validationText.PwayCode02.Text).WithErrorCode(validationText.PwayCode02.ErrorCode);
            RuleFor(r => r.PwayCode)
                .Must(m => !m.HasValue || m.Value == 0).When(m => m.ProgType == 25)
                    .WithMessage(validationText.PwayCode03.Text).WithErrorCode(validationText.PwayCode03.ErrorCode);

            RuleFor(r => r.StdCode)
                .LessThanOrEqualTo(99999).WithMessage(validationText.StdCode01.Text).WithErrorCode(validationText.StdCode01.ErrorCode)
                .Must(m => m.HasValue && m.Value > 0).When(m => m.ProgType == 25).WithMessage(validationText.StdCode02.Text).WithErrorCode(validationText.StdCode02.ErrorCode);

            RuleFor(r => r.StdCode)
                .Must(m => !m.HasValue || m.Value == 0).When(m => inList(m.ProgType, new[] { 2, 3, 20, 21, 22, 23 }))
                    .WithMessage(validationText.StdCode03.Text).WithErrorCode(validationText.StdCode03.ErrorCode);

            RuleFor(r => r.DateOfBirth)
                .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage(validationText.DateOfBirth02.Text).WithErrorCode(validationText.DateOfBirth02.ErrorCode);

            RuleFor(r => r.LearnStartDate)
                .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage(validationText.LearnStartDate02.Text).WithErrorCode(validationText.LearnStartDate02.ErrorCode);

            RuleFor(r => r.LearnPlanEndDate)
                .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage(validationText.LearnPlanEndDate02.Text).WithErrorCode(validationText.LearnPlanEndDate02.ErrorCode);
        }
    }
}