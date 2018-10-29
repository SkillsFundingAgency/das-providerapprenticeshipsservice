using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipPriceHistory
{
    public class GetApprenticeshipPriceHistoryQueryRequest: IRequest<GetApprenticeshipPriceHistoryQueryResponse>
    {
        public long ApprenticeshipId { get; set; }

        public long ProviderId { get; set; }
    }
}
