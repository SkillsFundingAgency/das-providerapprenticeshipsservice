using FluentValidation.Attributes;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    [Validator(typeof(VerificationOfRelationshipViewModelValidator))]
    public class VerificationOfRelationshipViewModel
    {
        public long ProviderId { get; set; }
        public string HashedCommitmentId { get; set; }
        public string LegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public string LegalEntityAddress { get; set; }
        public OrganisationType LegalEntityOrganisationType { get; set; }
        public bool? OrganisationIsSameOrConnected { get; set; }
    }
}