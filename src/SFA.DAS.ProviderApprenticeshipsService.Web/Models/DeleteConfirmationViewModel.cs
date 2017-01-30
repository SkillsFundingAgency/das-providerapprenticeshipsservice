using FluentValidation.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    [Validator(typeof(DeleteConfirmationViewModelValidator))]
    public sealed class DeleteConfirmationViewModel
    {
        public long ProviderId { get; set; }
        public string HashedCommitmentId { get; set; }
        public string HashedApprenticeshipId { get; set; }
    }
}