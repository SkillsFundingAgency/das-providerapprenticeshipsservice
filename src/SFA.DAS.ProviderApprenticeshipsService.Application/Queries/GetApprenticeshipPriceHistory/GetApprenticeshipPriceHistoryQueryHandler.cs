using System.Linq;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipPriceHistory
{
    public class GetApprenticeshipPriceHistoryQueryHandler : IAsyncRequestHandler<GetApprenticeshipPriceHistoryQueryRequest, GetApprenticeshipPriceHistoryQueryResponse>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;

        public GetApprenticeshipPriceHistoryQueryHandler(IProviderCommitmentsApi commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
        }

        public async Task<GetApprenticeshipPriceHistoryQueryResponse> Handle(GetApprenticeshipPriceHistoryQueryRequest message)
        {
            var response = await _commitmentsApi.GetPriceHistory(message.ProviderId, message.ApprenticeshipId);

            return new GetApprenticeshipPriceHistoryQueryResponse
            {
                History = response.ToList()
            };
        }
    }
}
