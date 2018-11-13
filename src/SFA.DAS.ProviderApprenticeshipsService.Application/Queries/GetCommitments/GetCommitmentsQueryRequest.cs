using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitments
{
    public class GetCommitmentsQueryRequest : IRequest<GetCommitmentsQueryResponse>
    {
        public long ProviderId { get; set; }
    }
}