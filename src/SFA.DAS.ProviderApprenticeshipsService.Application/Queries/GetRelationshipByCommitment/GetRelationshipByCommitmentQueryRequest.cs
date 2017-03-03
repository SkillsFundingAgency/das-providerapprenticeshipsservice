using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetRelationshipByCommitment
{
    public class GetRelationshipByCommitmentQueryRequest : IAsyncRequest<GetRelationshipByCommitmentQueryResponse>
    {
        public long ProviderId { get; set; }
        public long CommitmentId { get; set; }
    }
}