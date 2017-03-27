using System;
using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class ApprovedApprenticeshipViewModelValidator  : AbstractValidator<ApprenticeshipViewModel>
    {
        private readonly IApprenticeshipValidationErrorText _errorText;

        public ApprovedApprenticeshipViewModelValidator(IApprenticeshipValidationErrorText errorText)
        {
            if (errorText == null)
                throw new ArgumentNullException(nameof(errorText));

            _errorText = errorText;

            RuleFor(x => x.Cost).NotEmpty().WithMessage(_errorText.TrainingPrice01.Text);
        }
    }
}