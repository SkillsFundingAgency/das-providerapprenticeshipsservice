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
        public CsvRecordValidator()
        {
            var text = new ApprenticeshipValidationText();
            Func<int?, IEnumerable<int>, bool> inList = (v, l) => !v.HasValue || l.Contains(v.Value);

            RuleFor(r => r.ProgType)
                .NotEmpty().WithMessage(text.ProgType01.Text).WithErrorCode(text.ProgType01.ErrorCode)
                .LessThanOrEqualTo(99).WithMessage(text.ProgType01.Text).WithErrorCode(text.ProgType01.ErrorCode)
                .Must(m => inList(m, new [] { 2, 3, 20, 21, 22, 23, 25})).WithMessage(text.ProgType02.Text).WithErrorCode(text.ProgType02.ErrorCode);

            RuleFor(r => r.FworkCode)
                .LessThanOrEqualTo(999).WithMessage(text.FworkCode01.Text).WithErrorCode(text.FworkCode01.ErrorCode)
                .Must(m => (m ?? 0) > 0).When(m => m.ProgType.HasValue && inList(m.ProgType, new[] { 2, 3, 20, 21, 22, 23 })).WithMessage(text.FworkCode02MustFworkCode.Text).WithErrorCode(text.FworkCode02MustFworkCode.ErrorCode)
                ;

            RuleFor(r => r.FworkCode)
                .Must(m => !m.HasValue || m.Value == 0).When(m => m.ProgType == 25).WithMessage(text.FworkCode03MustNotFworkCode.Text).WithErrorCode(text.FworkCode03MustNotFworkCode.ErrorCode)
                ;

            RuleFor(r => r.PwayCode)
                .LessThanOrEqualTo(999).When(m => inList(m.ProgType, new[] { 2, 3, 20, 21, 22, 23 })).WithMessage(text.PwayCode01.Text).WithErrorCode(text.PwayCode01.ErrorCode)
                .Must(m => (m ?? 0) > 0).When(m => m.ProgType.HasValue && inList(m.ProgType, new[] { 2, 3, 20, 21, 22, 23 })).WithMessage(text.PwayCode02.Text).WithErrorCode(text.PwayCode02.ErrorCode)
                ;
            RuleFor(r => r.PwayCode)
                .Must(m => !m.HasValue || m.Value == 0).When(m => m.ProgType == 25)
                    .WithMessage(text.PwayCode03.Text).WithErrorCode(text.PwayCode03.ErrorCode)
                ;

            RuleFor(r => r.StdCode)
                .LessThanOrEqualTo(99999).WithMessage(text.StdCode01.Text).WithErrorCode(text.StdCode01.ErrorCode)
                .Must(m => m.HasValue && m.Value > 0).When(m => m.ProgType == 25).WithMessage(text.StdCode02.Text).WithErrorCode(text.StdCode02.ErrorCode)
                ;
            RuleFor(r => r.StdCode)
                .Must(m => !m.HasValue || m.Value == 0).When(m => inList(m.ProgType, new[] { 2, 3, 20, 21, 22, 23 }))
                    .WithMessage(text.StdCode03.Text).WithErrorCode(text.StdCode03.ErrorCode)
                ;

            RuleFor(r => r.DateOfBirth)
                .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage(text.DateOfBirth02.Text).WithErrorCode(text.DateOfBirth02.ErrorCode);
        }
    }
}