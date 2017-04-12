using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.ApprenticeshipUpdate
{
    public class CreateApprenticeshipUpdateViewModelValidator : AbstractValidator<CreateApprenticeshipUpdateViewModel>
    {
        public CreateApprenticeshipUpdateViewModelValidator()
        {
            RuleFor(x => x.ChangesConfirmed).NotEmpty().WithMessage("Select an option");
        }
    }
}