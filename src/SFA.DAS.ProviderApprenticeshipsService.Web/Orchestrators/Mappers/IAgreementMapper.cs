using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public interface IAgreementMapper
    {
        CommitmentAgreement Map(Commitments.Api.Types.Commitment.CommitmentAgreement commitmentAgreement);
    }
}