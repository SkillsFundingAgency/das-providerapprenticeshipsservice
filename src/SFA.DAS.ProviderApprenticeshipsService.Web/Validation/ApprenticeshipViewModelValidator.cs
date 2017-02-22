using System;
using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
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

        protected override void ValidateUln()
        {
            When(x => !string.IsNullOrEmpty(x.ULN), () =>
            {
                base.ValidateUln();
            });
        }

        protected override void ValidateDateOfBirth()
        {
            When(x => HasAnyValuesSet(x.DateOfBirth), () => 
            {
                base.ValidateDateOfBirth();
            });
        }

        protected override void ValidateStartDate()
        {
            When(x => HasYearOrMonthValueSet(x.StartDate), () =>
            {
                base.ValidateStartDate();
            });
            
        }

        protected override void ValidateEndDate()
        {
            When(x => HasYearOrMonthValueSet(x.EndDate), () =>
            {
                base.ValidateEndDate();
            });
        }

        protected override void ValidateCost()
        {
            When(x => !string.IsNullOrEmpty(x.Cost), () => 
            {
                base.ValidateCost();
            });
        }

        private bool HasYearOrMonthValueSet(DateTimeViewModel date)
        {
            if (date == null) return false;

            if (date.Day.HasValue || date.Month.HasValue || date.Year.HasValue) return true;

            return false;
        }

        private bool HasAnyValuesSet(DateTimeViewModel dateOfBirth)
        {
            if (dateOfBirth == null) return false;

            if (dateOfBirth.Day.HasValue || dateOfBirth.Month.HasValue || dateOfBirth.Year.HasValue) return true;

            return false;
        }
    }
}