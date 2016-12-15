using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class ApprenticeshipViewModelApproveValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipViewModelApproveValidator()
        {
            RuleFor(r => r.FirstName).NotEmpty();
            RuleFor(r => r.LastName).NotEmpty();
            RuleFor(r => r.ULN).NotEmpty();
            RuleFor(r => r.Cost).NotEmpty();

            RuleFor(r => r.StartDate).NotNull();
            RuleFor(r => r.EndDate).NotNull();

            RuleFor(r => r.TrainingCode).NotEmpty();

            RuleFor(r => r.DateOfBirth).NotNull();

            RuleFor(r => r.NINumber).NotEmpty();
        }
    }
}