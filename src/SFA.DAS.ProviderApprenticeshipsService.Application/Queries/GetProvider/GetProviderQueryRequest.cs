using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider
{
    public class GetProviderQueryRequest : IAsyncRequest<GetProviderQueryResponse>
    {
        public int UKPRN { get; set; }
    }
}