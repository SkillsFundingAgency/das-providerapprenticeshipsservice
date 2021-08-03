using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Validation.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;
using SFA.DAS.Learners.Validators;
using SFA.DAS.ProviderApprenticeshipsService.Application.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class ApprenticeshipCoreValidator : AbstractValidator<ApprenticeshipViewModel>, IApprenticeshipCoreValidator
    {
        protected static readonly Func<string, int, bool> LengthLessThanFunc = (str, length) => (str?.Length ?? length) < length;
        protected readonly IApprenticeshipValidationErrorText ValidationText;
        protected readonly ICurrentDateTime CurrentDateTime;
        private readonly IAcademicYearDateProvider _academicYear;
        private readonly IUlnValidator _ulnValidator;
        protected readonly IMediator Mediator;

        public ApprenticeshipCoreValidator(IApprenticeshipValidationErrorText validationText, 
                                            ICurrentDateTime currentDateTime, 
                                            IAcademicYearDateProvider academicYear,
                                            IUlnValidator ulnValidator,
                                            IMediator mediator)
        {
            ValidationText = validationText;
            CurrentDateTime = currentDateTime;
            _academicYear = academicYear;
            _ulnValidator = ulnValidator;
            Mediator = mediator;

            ValidateFirstName();

            ValidateLastName();

            ValidateUln();

            ValidateTraining();

            ValidateDateOfBirth();

            ValidateStartDate();

            ValidateEndDate();

            ValidateCost();

            ValidateProviderReference();

            ValidateAgreementId();
        }

        private void ValidateFirstName()
        {
            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage(ValidationText.GivenNames01.Text).WithErrorCode(ValidationText.GivenNames01.ErrorCode)
                .Must(m => LengthLessThanFunc(m, 101)).WithMessage(ValidationText.GivenNames02.Text).WithErrorCode(ValidationText.GivenNames02.ErrorCode);
        }

        private void ValidateLastName()
        {
            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage(ValidationText.FamilyName01.Text).WithErrorCode(ValidationText.FamilyName01.ErrorCode)
                .Must(m => LengthLessThanFunc(m, 101)).WithMessage(ValidationText.FamilyName02.Text).WithErrorCode(ValidationText.FamilyName02.ErrorCode); ;
        }

        protected virtual void ValidateUln()
        {
            RuleFor(x => x.ULN)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(BeValidTenDigitUlnNumber).WithMessage(ValidationText.Uln01.Text).WithErrorCode(ValidationText.Uln01.ErrorCode)
                .Must(BeValidUlnNumber).WithMessage(ValidationText.Uln03.Text).WithErrorCode(ValidationText.Uln03.ErrorCode);
        }

        protected virtual void ValidateTraining()
        {
            RuleFor(x => x.CourseCode)
                .NotEmpty().WithMessage(ValidationText.CourseCode01.Text).WithErrorCode(ValidationText.CourseCode01.ErrorCode); ;
        }

        protected virtual void ValidateDateOfBirth()
        {
            RuleFor(r => r.DateOfBirth)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage(ValidationText.DateOfBirth01.Text).WithErrorCode(ValidationText.DateOfBirth01.ErrorCode)
                .Must(ValidateDateOfBirth).WithMessage(ValidationText.DateOfBirth01.Text).WithErrorCode(ValidationText.DateOfBirth01.ErrorCode)
                .Must(WillApprenticeBeAtLeast15AtStartOfTraining).WithMessage(ValidationText.DateOfBirth02.Text).WithErrorCode(ValidationText.DateOfBirth02.ErrorCode)
                .Must(WillApprenticeBeNoMoreThan115AtTheStartOfTheCurrentTeachingYear).WithMessage(ValidationText.DateOfBirth06.Text).WithErrorCode(ValidationText.DateOfBirth06.ErrorCode);
        }

        protected virtual void ValidateStartDate()
        {
            RuleFor(x => x.StartDate)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .MustAsync(async (viewModel, startDate, context, cancellationToken) => await TrainingCourseValidOnStartDate(viewModel, startDate, context))
                    .WithErrorCode(ValidationText.LearnStartDateNotValidForTrainingCourse.ErrorCode)
                    .WithMessage(ValidationText.LearnStartDateNotValidForTrainingCourse.Text)
                .NotNull().WithMessage(ValidationText.LearnStartDate01.Text).WithErrorCode(ValidationText.LearnStartDate01.ErrorCode)
                .Must(ValidateDateWithoutDay).WithMessage(ValidationText.LearnStartDate01.Text).WithErrorCode(ValidationText.LearnStartDate01.ErrorCode)
                .Must(StartDateForTransferNotBeforeMay2018).WithMessage(ValidationText.LearnStartDate06.Text).WithErrorCode(ValidationText.LearnStartDate06.ErrorCode)
                .Must(NotBeBeforeMay2017).WithMessage(ValidationText.LearnStartDate02.Text).WithErrorCode(ValidationText.LearnStartDate02.ErrorCode)
                .Must(StartDateWithinAYearOfTheEndOfTheCurrentTeachingYear).WithMessage(ValidationText.LearnStartDate05.Text).WithErrorCode(ValidationText.LearnStartDate05.ErrorCode);
                //.Must(BeWithinAcademicYearFundingPeriod).WithMessage(_validationText.AcademicYearStartDate01.Text).WithErrorCode(_validationText.AcademicYearStartDate01.ErrorCode);
        }

        protected virtual void ValidateEndDate()
        {
            RuleFor(x => x.EndDate)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage(ValidationText.LearnPlanEndDate01.Text).WithErrorCode(ValidationText.LearnPlanEndDate01.ErrorCode)
                .Must(ValidateDateWithoutDay).WithMessage(ValidationText.LearnPlanEndDate01.Text).WithErrorCode(ValidationText.LearnPlanEndDate01.ErrorCode)
                .Must(BeGreaterThenStartDate).WithMessage(ValidationText.LearnPlanEndDate02.Text).WithErrorCode(ValidationText.LearnPlanEndDate02.ErrorCode);
        }

        public KeyValuePair<string, string>? CheckEndDateInFuture(DateTimeViewModel endDate)
        {
            const string endDateKey = "EndDate";

            var now = CurrentDateTime.Now;
            return new DateTime(endDate.Year.Value, endDate.Month.Value, 1) > new DateTime(now.Year, now.Month, 1)
                ? (KeyValuePair<string, string>?)null
                : new KeyValuePair<string, string>(endDateKey, ValidationText.LearnPlanEndDate03.Text);
        }

        protected virtual void ValidateCost()
        {
            decimal parsed;

            RuleFor(x => x.Cost)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage(ValidationText.TrainingPrice01.Text).WithErrorCode(ValidationText.TrainingPrice01.ErrorCode)
                .Matches("^([1-9]{1}([0-9]{1,2})?)+(,[0-9]{3})*$|^[1-9]{1}[0-9]*$").WithMessage(ValidationText.TrainingPrice01.Text).WithErrorCode(ValidationText.TrainingPrice01.ErrorCode)
                .Must(m => decimal.TryParse(m, out parsed) && parsed <= 100000).WithMessage(ValidationText.TrainingPrice02.Text).WithErrorCode(ValidationText.TrainingPrice02.ErrorCode);
        }

        public Dictionary<string, string> MapOverlappingErrors(GetOverlappingApprenticeshipsQueryResponse overlappingErrors)
        {
            var dict = new Dictionary<string, string>();
            const string startText = "The start date is not valid";
            const string endText = "The end date is not valid";

            //todo: store against real StartDate and EndDate fields and handle in the same manner as employer comitments
            const string startDateKey = "StartDateOverlap";
            const string endDateKey = "EndDateOverlap";

            foreach (var item in overlappingErrors.GetFirstOverlappingApprenticeships())
            {
                switch (item.ValidationFailReason)
                {
                    case ValidationFailReason.OverlappingStartDate:
                        dict.AddIfNotExists(startDateKey, startText);
                        break;
                    case ValidationFailReason.OverlappingEndDate:
                        dict.AddIfNotExists(endDateKey, endText);
                        break;
                    case ValidationFailReason.DateEmbrace:
                        dict.AddIfNotExists(startDateKey, startText);
                        dict.AddIfNotExists(endDateKey, endText);
                        break;
                    case ValidationFailReason.DateWithin:
                        dict.AddIfNotExists(startDateKey, startText);
                        dict.AddIfNotExists(endDateKey, endText);
                        break;
                }
            }
            return dict;
        }

        private void ValidateProviderReference()
        {
            RuleFor(x => x.ProviderRef)
                .Must(m => LengthLessThanFunc(m, 21))
                    .When(x => !string.IsNullOrEmpty(x.ProviderRef)).WithMessage(ValidationText.ProviderRef01.Text).WithErrorCode(ValidationText.ProviderRef01.ErrorCode);
        }

        private void ValidateAgreementId()
        {
            RuleFor(x => x.AgreementId)
               .Cascade(CascadeMode.StopOnFirstFailure)
               .NotEmpty().WithMessage(ValidationText.AgreementIdBlank.Text).WithErrorCode(ValidationText.AgreementIdBlank.ErrorCode);
        }

        private bool WillApprenticeBeAtLeast15AtStartOfTraining(ApprenticeshipViewModel model, DateTimeViewModel dob)
        {
            DateTime? startDate = model?.StartDate?.DateTime;
            DateTime? dobDate = dob?.DateTime;

            if (startDate == null || dob == null) return true; // Don't fail validation if both fields not set

            int age = startDate.Value.Year - dobDate.Value.Year;
            if (startDate < dobDate.Value.AddYears(age)) age--;

            return age >= 15;
        }

        private bool WillApprenticeBeNoMoreThan115AtTheStartOfTheCurrentTeachingYear(DateTimeViewModel dob)
        {
            var age = _academicYear.CurrentAcademicYearStartDate.Year - dob.DateTime.Value.Year;
            return age <= 115;
        }

        private bool StartDateWithinAYearOfTheEndOfTheCurrentTeachingYear(DateTimeViewModel startDate)
        {
            return startDate.DateTime.Value <= _academicYear.CurrentAcademicYearEndDate.AddYears(1);
        }
        private bool StartDateForTransferNotBeforeMay2018(ApprenticeshipViewModel viewModel, DateTimeViewModel date)
        {
            if (!viewModel.IsPaidForByTransfer || date.DateTime >= new DateTime(2018, 5, 1))
            {
                return true;
            }

            //Add alternative detail error message to dummy property
            viewModel.StartDateTransfersMinDateAltDetailMessage = "The start date can't be earlier than May 2018";
            return false;
        }
        private bool BeGreaterThenStartDate(ApprenticeshipViewModel viewModel, DateTimeViewModel date)
        {
            if (viewModel.StartDate?.DateTime == null || viewModel.EndDate?.DateTime == null) return true;

            return viewModel.StartDate.DateTime < viewModel.EndDate.DateTime;
        }

        private bool ValidateDateWithoutDay(DateTimeViewModel date)
        {
            return date.DateTime != null;
        }

        private bool NotBeBeforeMay2017(DateTimeViewModel date)
        {
            return date.DateTime >= new DateTime(2017, 5, 1);
        }

        private bool ValidateDateOfBirth(DateTimeViewModel date)
        {
            // Check the day has value as the view model supports just month and year entry
            return date.DateTime != null && date.Day.HasValue;
        }

        private bool BeValidTenDigitUlnNumber(string uln)
        {
            var result = _ulnValidator.Validate(uln);

            return result != UlnValidationResult.IsInValidTenDigitUlnNumber && result != UlnValidationResult.IsEmptyUlnNumber;
        }

        private bool BeValidUlnNumber(string uln)
        {
            return _ulnValidator.Validate(uln) != UlnValidationResult.IsInvalidUln;
        }

        private async Task<bool> TrainingCourseValidOnStartDate(ApprenticeshipViewModel viewModel, DateTimeViewModel startDate, PropertyValidatorContext context)
        {
            if (viewModel.IsContinuation)
                return true;

            if (string.IsNullOrWhiteSpace(viewModel.CourseCode) || (!startDate.DateTime.HasValue))
                return true;

            var result = await Mediator.Send(new GetTrainingProgrammesQueryRequest
            {
                EffectiveDate = null,
                IncludeFrameworks = true
            });

            var course = result.TrainingProgrammes.Single(x => x.CourseCode == viewModel.CourseCode);

            var courseStatus = course.GetStatusOn(startDate.DateTime.Value);

            if (courseStatus == TrainingProgrammeStatus.Active)
                return true;

            var suffix = courseStatus == TrainingProgrammeStatus.Pending
                ? $"after {course.EffectiveFrom.Value.AddMonths(-1):MM yyyy}"
                : $"before {course.EffectiveTo.Value.AddMonths(1):MM yyyy}";

            context.MessageFormatter.AppendArgument("suffix", suffix);
            return false;
        }
    }
}