using System;
using System.Collections.Generic;
using System.Linq;

using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class CsvRecordValidator : AbstractValidator<CsvRecord>
    {
        public CsvRecordValidator()
        {
            //Func<string, int, bool> lengthLessThan = (str, lenth) => (str?.Length ?? 0) <= lenth;
            Func<int?, IEnumerable<int>, bool> inList = (v, l) => !v.HasValue || l.Contains(v.Value);

            RuleFor(r => r.ProgType)
                .NotEmpty().WithMessage("Prog type cannot be empty").WithErrorCode("ProgType_01")
                .LessThanOrEqualTo(99).WithMessage("Cannot be greater than 99").WithErrorCode("ProgType_01")
                .Must(m => inList(m, new [] { 2, 3, 20, 21, 22, 23, 25})).WithMessage("Prog type must be any one of 2, 3, 20, 21, 22, 23, 25").WithErrorCode("ProgType_02");

            RuleFor(r => r.FworkCode)
                .LessThanOrEqualTo(999).WithMessage("Framework code must be less than 1000 characters").WithErrorCode("FworkCode_01")
                .Must(m => (m ?? 0) > 0).When(m => m.ProgType.HasValue && inList(m.ProgType, new[] { 2, 3, 20, 21, 22, 23 })).WithMessage("Framework code must be greater then 0 for this prog type").WithErrorCode("FworkCode_02")
                ;

            RuleFor(r => r.FworkCode)
                .Must(m => !m.HasValue || m.Value == 0).When(m => m.ProgType == 25).WithMessage("Framework code must be empty when prog type is 25").WithErrorCode("FworkCode_03")
                ;

            RuleFor(r => r.PwayCode)
                .LessThanOrEqualTo(999).When(m => inList(m.ProgType, new[] { 2, 3, 20, 21, 22, 23 })).WithMessage("Pathway code must be less than 1000 characters").WithErrorCode("PwayCode_01")
                .Must(m => (m ?? 0) > 0).When(m => m.ProgType.HasValue && inList(m.ProgType, new[] { 2, 3, 20, 21, 22, 23 })).WithMessage("Pathway code must be greater then 0 for this prog type").WithErrorCode("PwayCode_02")
                ;
            RuleFor(r => r.PwayCode)
                .Must(m => !m.HasValue || m.Value == 0).When(m => m.ProgType == 25).WithMessage("Pathway code must be empty when prog type is 25").WithErrorCode("PwayCode_03")
                ;

            RuleFor(r => r.StdCode)
                .LessThanOrEqualTo(99999).WithMessage("Standard code must be less than 100000 characters").WithErrorCode("StdCode_01")
                .Must(m => m.HasValue && m.Value > 0).When(m => m.ProgType == 25).WithMessage("Standard code must be greater then 0 for this prog type").WithErrorCode("StdCode_02")
                ;
            RuleFor(r => r.StdCode)
                .Must(m => !m.HasValue || m.Value == 0).When(m => inList(m.ProgType, new[] { 2, 3, 20, 21, 22, 23 })).WithMessage("Standard code must be empty for this prog type").WithErrorCode("StdCode_03")
                ;
        }
    }
}