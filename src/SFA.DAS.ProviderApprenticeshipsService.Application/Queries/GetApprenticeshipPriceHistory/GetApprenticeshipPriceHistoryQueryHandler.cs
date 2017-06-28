using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipPriceHistory
{
    public class GetApprenticeshipPriceHistoryQueryHandler : IAsyncRequestHandler<GetApprenticeshipPriceHistoryQueryRequest, GetApprenticeshipPriceHistoryQueryResponse>
    {
        private readonly IApprenticeshipApi _apprenticeshipApi;

        public GetApprenticeshipPriceHistoryQueryHandler(IApprenticeshipApi apprenticeshipApi)
        {
            _apprenticeshipApi = apprenticeshipApi;
        }

        public async Task<GetApprenticeshipPriceHistoryQueryResponse> Handle(GetApprenticeshipPriceHistoryQueryRequest message)
        {
            var response = await _apprenticeshipApi.GetPriceHistory(message.ApprenticeshipId);

            return new GetApprenticeshipPriceHistoryQueryResponse
            {
                History = response.ToList()
            };
        }
    }
}
