using System;
using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public sealed class ApprenticeshipViewModelValidator : ApprenticeshipBaseValidator
    {
        public ApprenticeshipViewModelValidator() : this(new WebApprenticeshipValidationText())
        { } // The default is used by the MVC model binding

        public ApprenticeshipViewModelValidator(WebApprenticeshipValidationText validationText) : base(validationText)
        {
            var yesterday = DateTime.UtcNow.AddDays(-1);

            RuleFor(r => r.DateOfBirth)
                .Must(m => CheckIfNotNull(m?.DateTime, m?.DateTime < yesterday)).WithMessage(validationText.DateOfBirth03.Text).WithErrorCode(validationText.DateOfBirth03.ErrorCode);
        }
    }
}