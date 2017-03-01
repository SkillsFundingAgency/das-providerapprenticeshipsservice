using FluentValidation.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    [Validator(typeof(VerificationOfRelationshipViewModelValidator))]
    public class VerificationOfRelationshipViewModel
    {
        public long ProviderId { get; set; }
        public string HashedCommitmentId { get; set; }
        public string LegalEntityName { get; set; }
        public bool? OrganisationIsSameOrConnected { get; set; }
    }
}