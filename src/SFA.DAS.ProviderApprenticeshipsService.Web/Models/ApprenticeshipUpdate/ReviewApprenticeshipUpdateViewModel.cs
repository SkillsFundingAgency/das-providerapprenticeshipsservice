using FluentValidation.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.ApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate
{
    [Validator(typeof(ReviewApprenticeshipUpdateViewModelValidator))]
    public class ReviewApprenticeshipUpdateViewModel : ApprenticeshipUpdateViewModel
    {
        public bool? ApproveChanges { get; set; }
    }
}