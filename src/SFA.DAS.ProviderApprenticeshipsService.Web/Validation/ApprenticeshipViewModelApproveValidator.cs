using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class ApprenticeshipViewModelApproveValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipViewModelApproveValidator()
        {
            RuleFor(r => r.ULN).NotEmpty();
            RuleFor(r => r.Cost).NotEmpty();

            RuleFor(r => r.StartMonth).NotEmpty();
            RuleFor(r => r.StartYear).NotEmpty();
            RuleFor(r => r.EndMonth).NotEmpty();
            RuleFor(r => r.EndYear).NotEmpty();

            RuleFor(r => r.TrainingCode).NotEmpty();

            RuleFor(r => r.DateOfBirthDay).NotEmpty();
            RuleFor(r => r.DateOfBirthMonth).NotEmpty();
            RuleFor(r => r.DateOfBirthYear).NotEmpty();

            RuleFor(r => r.NINumber).NotEmpty();
        }
    }
}