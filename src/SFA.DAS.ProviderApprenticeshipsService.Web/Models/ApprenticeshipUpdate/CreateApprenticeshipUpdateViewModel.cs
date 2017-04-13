using FluentValidation.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.ApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate
{
    [Validator(typeof(CreateApprenticeshipUpdateViewModelValidator))]
    public class CreateApprenticeshipUpdateViewModel : ApprenticeshipUpdateViewModel
    {
        public bool? ChangesConfirmed { get; set; }
    }
}