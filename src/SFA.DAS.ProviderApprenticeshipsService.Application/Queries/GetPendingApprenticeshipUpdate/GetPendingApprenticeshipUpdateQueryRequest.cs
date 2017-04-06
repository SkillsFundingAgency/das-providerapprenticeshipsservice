using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetPendingApprenticeshipUpdate
{
    public class GetPendingApprenticeshipUpdateQueryRequest: IAsyncRequest<GetPendingApprenticeshipUpdateQueryResponse>
    {
        public long ProviderId { get; set; }
        public long ApprenticeshipId { get; set; }
    }
}
