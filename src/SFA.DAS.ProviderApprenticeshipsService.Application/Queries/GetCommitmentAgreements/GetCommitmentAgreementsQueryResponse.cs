using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements;

public class GetCommitmentAgreementsQueryResponse
{
    public List<ProviderCommitmentAgreement> CommitmentAgreements { get; set; }
}