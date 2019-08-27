using System;
using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class ApprovedApprenticeshipViewModelValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        private readonly IApprenticeshipValidationErrorText _errorText;

        public ApprovedApprenticeshipViewModelValidator(IApprenticeshipValidationErrorText errorText)
        {
            if (errorText == null)
                throw new ArgumentNullException(nameof(errorText));

            _errorText = errorText;

            RuleFor(x => x.DateOfBirth).Must(NotEmptyDate)
                .WithMessage(_errorText.DateOfBirth01.Text);

            RuleFor(x => x.ULN).NotEmpty().OverridePropertyName("ULN")
                .WithMessage(_errorText.Uln01.Text);

            RuleFor(x => x.CourseCode).NotEmpty()
                .WithMessage(_errorText.CourseCode01.Text);

            RuleFor(x=> x.StartDate).Must(NotEmptyDate)
                .WithMessage(_errorText.LearnStartDate01.Text);

            RuleFor(x => x.EndDate).Must(NotEmptyDate)
                .WithMessage(_errorText.LearnPlanEndDate01.Text);

            RuleFor(x => x.Cost).NotEmpty().OverridePropertyName("Cost")
                .WithMessage(_errorText.TrainingPrice01.Text);
        }

        private bool NotEmptyDate(DateTimeViewModel dateTimeViewModel)
        {
            return dateTimeViewModel.Day.HasValue
                || dateTimeViewModel.Month.HasValue
                || dateTimeViewModel.Year.HasValue;
        }
    }
}