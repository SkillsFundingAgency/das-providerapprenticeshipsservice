using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class ApprenticeshipViewModelApproveValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipViewModelApproveValidator(IApprenticeshipValidationErrorText textValidation)
        {
            RuleFor(r => r.ULN)
                .NotEmpty().WithMessage(textValidation.Uln01.Text).WithErrorCode(textValidation.Uln01.ErrorCode);

            RuleFor(r => r.Cost).NotEmpty().WithMessage(textValidation.TrainingPrice01.Text).WithErrorCode(textValidation.TrainingPrice01.ErrorCode);

            RuleFor(r => r.DateOfBirth)
                .Must(m => m?.DateTime != null)
                    .WithMessage(textValidation.DateOfBirth01.Text).WithErrorCode(textValidation.DateOfBirth01.ErrorCode);

            RuleFor(r => r.StartDate)
                .Must(m => m?.DateTime != null).WithMessage(textValidation.LearnStartDate01.Text).WithErrorCode(textValidation.LearnStartDate01.ErrorCode);
            RuleFor(r => r.EndDate)
                .Must(m => m?.DateTime != null).WithMessage(textValidation.LearnPlanEndDate01.Text).WithErrorCode(textValidation.LearnPlanEndDate01.ErrorCode);

            RuleFor(r => r.TrainingCode).NotEmpty().WithMessage(textValidation.TrainingCode01.Text).WithErrorCode(textValidation.TrainingCode01.ErrorCode);
        }
    }
}