using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprovedApprenticeship
{
    public class GetApprovedApprenticeshipQueryRequest : IAsyncRequest<GetApprovedApprenticeshipQueryResponse>
    {
        public long ApprovedApprenticeshipId { get; set; }
        public long ProviderId { get; set; }
    }
}
