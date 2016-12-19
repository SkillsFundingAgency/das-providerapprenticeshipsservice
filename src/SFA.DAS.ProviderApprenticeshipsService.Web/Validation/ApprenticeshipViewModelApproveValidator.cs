using System;

using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class ApprenticeshipViewModelApproveValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipViewModelApproveValidator()
        {
            Func<string, int, bool> lengthLessThan = (str, lenth) => (str?.Length ?? 0) <= lenth;

            RuleFor(r => r.ULN)
                .NotEmpty().WithErrorCode("ULN_01")
                .When(m => m.ULN == "9999999999").WithErrorCode("ULN_02")
                //.Must(Checksumvalidation).WithErrorCode("ULN_03")
                //.Must(The ULN is already in use on another record for this LearnStartDate).WithErrorCode("ULN_04")
                ;

            RuleFor(r => r.Cost)
                .NotNull().WithMessage("Cost cannot be enpty").WithErrorCode("TrainingPrice_01")
                .NotEmpty().Must(m => lengthLessThan(m, 6)).WithMessage("Cost must be less than one million").WithErrorCode("TrainingPrice_02");

            // ToDo: Add error code
            RuleFor(r => r.StartMonth).NotEmpty();
            RuleFor(r => r.StartYear).NotEmpty();
            RuleFor(r => r.EndMonth).NotEmpty();
            RuleFor(r => r.EndYear).NotEmpty();

            RuleFor(r => r.TrainingCode).NotEmpty();

            // ToDo: Add error code
            RuleFor(r => r.DateOfBirthDay).NotEmpty();
            RuleFor(r => r.DateOfBirthMonth).NotEmpty();
            RuleFor(r => r.DateOfBirthYear).NotEmpty();

            RuleFor(r => r.NINumber).NotNull().WithMessage("National insurance number must not be null").WithErrorCode("NINumber_01");
        }
    }
}