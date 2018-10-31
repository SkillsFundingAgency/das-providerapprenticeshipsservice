using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment
{
    public class GetCommitmentQueryRequest : IRequest<GetCommitmentQueryResponse>
    {
        public long ProviderId { get; set; }
        public long CommitmentId { get; set; }
    }
}