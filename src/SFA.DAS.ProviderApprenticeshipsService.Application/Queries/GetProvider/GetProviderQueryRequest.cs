using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider
{
    public class GetProviderQueryRequest : IRequest<GetProviderQueryResponse>
    {
        public int UKPRN { get; set; }
    }
}