using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types.Commitment;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements
{
    public class GetCommitmentAgreementsQueryResponse
    {
        public List<CommitmentAgreement> CommitmentAgreements { get; set; }
    }
}
