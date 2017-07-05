using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipPriceHistory
{
    public class GetApprenticeshipPriceHistoryQueryRequest: IAsyncRequest<GetApprenticeshipPriceHistoryQueryResponse>
    {
        public long ApprenticeshipId { get; set; }

        public long ProviderId { get; set; }
    }
}
