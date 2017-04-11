using FluentValidation.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.ApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate
{
    [Validator(typeof(UndoApprenticeshipUpdateViewModelValidator))]
    public class UndoApprenticeshipUpdateViewModel : ApprenticeshipUpdateViewModel
    {
        public bool? ConfirmUndo { get; set; }
    }
}