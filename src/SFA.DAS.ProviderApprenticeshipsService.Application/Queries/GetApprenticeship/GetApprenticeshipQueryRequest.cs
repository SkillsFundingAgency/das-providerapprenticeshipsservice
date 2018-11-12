using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship
{
    public class GetApprenticeshipQueryRequest : IRequest<GetApprenticeshipQueryResponse>
    {
        public long ProviderId { get; set; }

        public long ApprenticeshipId { get; set; }
    }
}