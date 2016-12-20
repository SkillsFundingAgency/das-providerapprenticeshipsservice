using System;
using System.Linq;
using System.Text.RegularExpressions;

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
                .NotEmpty().WithMessage("ULN cannot be empty").WithErrorCode("ULN_01");
                //.Must(Checksumvalidation).WithErrorCode("ULN_03")
                //.Must(The ULN is already in use on another record for this LearnStartDate).WithErrorCode("ULN_04")
                ;

            RuleFor(r => r.Cost)
                .NotEmpty().WithMessage("Cost cannot be empty").WithErrorCode("TrainingPrice_01")
                .Must(m => lengthLessThan(m, 6)).WithMessage("Cost must be less than one million").WithErrorCode("TrainingPrice_02");

            RuleFor(r => r.DateOfBirth)
                .Must(m => m.DateTime != null).WithMessage("Date of birth must not be empty");

            RuleFor(r => r.StartDate).NotNull();
            RuleFor(r => r.EndDate).NotNull();

            RuleFor(r => r.TrainingCode)
                .NotEmpty().WithMessage("Training code cannot be empty")
                .Must(ValidateStandardTrainingCode).WithMessage("Training code is not valid");

            RuleFor(r => r.NINumber)
                .Must(m => !string.IsNullOrEmpty(m)).WithMessage("National insurance number must not be null").WithErrorCode("NINumber_01");
        }

        private bool ValidateStandardTrainingCode(ApprenticeshipViewModel viewModel, string trainingCode)
        {
            if (string.IsNullOrWhiteSpace(trainingCode))
                return true;

            if (viewModel.ProgType.HasValue && viewModel.ProgType.Value == 25)
                return Regex.Match(trainingCode, @"\d").Success;

            var l = new[] { 2, 3, 20, 21, 22, 23 };
            if (viewModel.ProgType.HasValue && l.Contains(viewModel.ProgType.Value))
                return Regex.Match(trainingCode, @"\d-\d-\d").Success;

            return true;
        }

    }
}