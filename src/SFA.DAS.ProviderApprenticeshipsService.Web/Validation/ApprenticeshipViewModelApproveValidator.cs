﻿using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class ApprenticeshipViewModelApproveValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipViewModelApproveValidator()
        {
            var text = new Text.ApprenticeshipValidationText();

            RuleFor(r => r.ULN)
                .NotEmpty().WithMessage(text.Uln01.Text).WithErrorCode(text.Uln01.ErrorCode);
                ;

            RuleFor(r => r.Cost).NotEmpty().WithMessage("Enter the total agreed training cost");

            RuleFor(r => r.DateOfBirth)
                .Must(m => m?.DateTime != null)
                    .WithMessage(text.DateOfBirth01.Text).WithErrorCode(text.DateOfBirth01.ErrorCode);

            RuleFor(r => r.StartDate)
                .Must(m => m?.DateTime != null).WithMessage(text.LearnStartDate01.Text).WithErrorCode(text.LearnStartDate01.ErrorCode);
            RuleFor(r => r.EndDate)
                .Must(m => m?.DateTime != null).WithMessage(text.LearnPlanEndDate01.Text).WithErrorCode(text.LearnPlanEndDate01.ErrorCode);

            RuleFor(r => r.TrainingCode).NotEmpty().WithMessage("Training code cannot be empty");

            RuleFor(r => r.NINumber)
                .Must(m => !string.IsNullOrEmpty(m)).WithMessage("National insurance number must not be null").WithErrorCode("NINumber_01");

            RuleFor(r => r.DateOfBirth)
                .Must(m => m?.DateTime != null).WithMessage(text.DateOfBirth01.Text).WithErrorCode(text.DateOfBirth01.ErrorCode);
        }

    }
}