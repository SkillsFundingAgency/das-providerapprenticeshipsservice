using System;

using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    using System.Text.RegularExpressions;

    public sealed class ApprenticeshipViewModelValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipViewModelValidator()
        {
            var now = DateTime.Now;

            RuleFor(x => x.ULN)
                .Matches("^$|^[1-9]{1}[0-9]{9}$").WithMessage("Enter a valid unique learner number");

            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Enter a first name");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Enter a last name");

            RuleFor(x => x.NINumber)
                .Matches(@"^[abceghj-prstw-z][abceghj-nprstw-z]\d{6}[abcd]$", RegexOptions.IgnoreCase)
                .WithMessage("Enter a valid national insurance number");

            RuleFor(r => r.StartDate)
                .Must(m => _checkIfNotNull(m.ToDateTime(), m.ToDateTime() > now))
                .WithMessage("Learner start date must be in the future");

            RuleFor(r => r.EndDate)
                .Must(m => _checkIfNotNull(m.ToDateTime(), m.ToDateTime() > now))
                .WithMessage("Learner planed end date must be in the future");

            RuleFor(r => r.DateOfBirth)
                .Must(m => _checkIfNotNull(m.ToDateTime(), m.ToDateTime() < now))
                .WithMessage("Date of birth must be in the past");

            RuleFor(x => x.Cost).Matches("^$|^[1-9]{1}[0-9]*$").WithMessage("Enter the total agreed training cost");
        }

        private readonly Func<DateTime?, bool, bool> _checkIfNotNull = (dt, b) => dt == null || b ;
    }
}