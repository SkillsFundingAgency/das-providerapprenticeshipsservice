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
            var currentYear = DateTime.Now.Year;
            Func<string, int, bool> lengthLessThan = (str, lenth) => (str?.Length ?? 0) <= lenth;

            RuleFor(x => x.ULN).Matches("^$|^[1-9]{1}[0-9]{9}$").WithMessage("Enter a valid unique learner number");

            RuleFor(x => x.FirstName)
                .Must(m => !string.IsNullOrEmpty(m)).WithMessage("Enter a first name").WithErrorCode("GivenNames_01")
                .Must(m => lengthLessThan(m, 100)).WithMessage("First name cannot contain more then 100 chatacters").WithErrorCode("GivenNames_02");

            RuleFor(x => x.LastName)
                .Must(m => !string.IsNullOrEmpty(m)).WithMessage("Enter a last name").WithErrorCode("FamilyName_01")
                .Must(m => lengthLessThan(m, 100)).WithMessage("Last name cannot contain more then 100 chatacters").WithErrorCode("FamilyName_02");

            // ToDo: Add error code
            RuleFor(x => x.StartMonth).InclusiveBetween(1, 12).WithMessage("Enter a valid month for training start");
            RuleFor(x => x.StartYear).InclusiveBetween(currentYear, 9999).WithMessage("Enter a valid year for training start");
            RuleFor(x => x.EndMonth).InclusiveBetween(1, 12).WithMessage("Enter a valid month for training finish");
            RuleFor(x => x.EndYear).InclusiveBetween(currentYear, 9999).WithMessage("Enter a valid year for training finish");

            RuleFor(x => x.NINumber)
                .Must(m => lengthLessThan(m, 9)).WithMessage("National insurance number needs to be 10 characters long").WithErrorCode("NINumber_02")
                .Matches(@"^[abceghj-prstw-z][abceghj-nprstw-z]\d{6}[abcd\s]$", RegexOptions.IgnoreCase).WithMessage("Enter a valid national insurance number").WithErrorCode("NINumber_03");

            // ToDo: Add error code
            RuleFor(x => x.DateOfBirthDay)
                .InclusiveBetween(1, 31)
                .WithMessage("Enter a valid day for date of birth");
            RuleFor(x => x.DateOfBirthMonth)
                .InclusiveBetween(1, 12)
                .WithMessage("Enter a valid month for date of birth");
            RuleFor(x => x.DateOfBirthYear)
                .InclusiveBetween(1900, currentYear)
                .WithMessage("Enter a valid year for date of birth");
            RuleFor(x => x.Cost).Matches("^$|^[1-9]{1}[0-9]*$").WithMessage("Enter the total agreed training cost");

            RuleFor(x => x.ProviderRef)
                .Must(m => lengthLessThan(m, 20)).WithMessage("Provider reference must not contain more than 20 characters").WithErrorCode("ProvRef_01");
        }
    }
}