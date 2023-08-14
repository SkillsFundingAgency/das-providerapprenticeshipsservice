using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

public class AgreementMapper : IAgreementMapper
{
    public CommitmentAgreement Map(ProviderCommitmentAgreement commitmentAgreement)
    {
        return new CommitmentAgreement
        {
            // here we're basically mapping between what we call properties internally to what the view calls them
            AgreementID = commitmentAgreement.AccountLegalEntityPublicHashedId,                
            OrganisationName = commitmentAgreement.LegalEntityName
        };
    }
}