using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public class AgreementMapper : IAgreementMapper
    {
        public CommitmentAgreement Map(Commitments.Api.Types.Commitment.CommitmentAgreement commitmentAgreement)
        {
            return new CommitmentAgreement
            {
                // here we're basically mapping between what we call properties internally to what the view calls them
                AgreementID = commitmentAgreement.AccountLegalEntityPublicHashedId,
                CohortID = commitmentAgreement.Reference,
                OrganisationName = commitmentAgreement.LegalEntityName
            };
        }
    }
}