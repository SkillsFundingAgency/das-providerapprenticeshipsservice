using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

public interface IAgreementMapper
{
    CommitmentAgreement Map(ProviderCommitmentAgreement commitmentAgreement);
}