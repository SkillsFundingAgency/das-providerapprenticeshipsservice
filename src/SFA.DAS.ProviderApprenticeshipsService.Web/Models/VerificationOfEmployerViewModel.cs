using FluentValidation.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    [Validator(typeof(VerificationOfEmployerViewModelValidator))]
    public class VerificationOfEmployerViewModel
    {
        public long ProviderId { get; set; }
        public string HashedCommitmentId { get; set; }
        public string LegalEntityName { get; set; }
        public bool? ConfirmProvisionOfTrainingForOrganisation { get; set; }
    }
}