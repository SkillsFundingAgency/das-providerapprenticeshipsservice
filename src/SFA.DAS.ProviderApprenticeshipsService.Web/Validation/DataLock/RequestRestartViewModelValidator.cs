using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.DataLock
{
    public class RequestRestartViewModelValidator : AbstractValidator<RequestRestartViewModel>
    {
        public RequestRestartViewModelValidator()
        {
            RuleFor(x => x.SubmitStatusViewModel).NotEmpty().WithMessage("Select an option");
        }
    }
}